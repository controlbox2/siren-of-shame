﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using SirenOfShame.Lib.Exceptions;
using SirenOfShame.Lib.Helpers;
using SirenOfShame.Lib.Settings;
using log4net;
using SirenOfShame.Lib;

namespace TeamCityServices
{
    // http://confluence.jetbrains.net/display/TW/REST+API+Plugin
    public class TeamCityService
    {
        private static readonly ILog _log = MyLogManager.GetLogger(typeof(TeamCityService));

        public delegate void GetProjectsCompleteDelegate(TeamCityProject[] projects);

        public delegate void GetBuildDefinitionsCompleteDelegate(TeamCityBuildDefinition[] buildDefinitions);

        public void GetProjects(string rootUrl, string userName, string password, GetProjectsCompleteDelegate complete, Action<Exception> onError)
        {
            WebClient webClient = new WebClient
            {
                Credentials = new NetworkCredential(userName, password)
            };
            rootUrl = GetRootUrl(rootUrl);
            var projectUrl = new Uri(rootUrl + "/httpAuth/app/rest/projects");
            webClient.DownloadStringCompleted += (s, e) =>
                {
                    try
                    {
                        XDocument doc = XDocument.Parse(e.Result);
                        if (doc.Root == null)
                        {
                            throw new Exception("Could not get project list");
                        }
                        TeamCityProject[] projects = doc.Root
                            .Elements("project")
                            .Select(projectXml => new TeamCityProject(rootUrl, projectXml))
                            .ToArray();
                        complete(projects);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error connecting to server", ex);
                        onError(ex);
                    }
                };
            webClient.DownloadStringAsync(projectUrl);
        }

        private string GetRootUrl(string rootUrl)
        {
            if (string.IsNullOrEmpty(rootUrl)) return null;
            rootUrl = rootUrl.TrimEnd('/');
            return rootUrl;
        }

        public void GetBuildDefinitions(TeamCityProject project, string userName, string password, GetBuildDefinitionsCompleteDelegate complete)
        {
            WebClient webClient = new WebClient
            {
                Credentials = new NetworkCredential(userName, password)
            };
            var projectDetailsUrl = new Uri(project.RootUrl + project.Href);
            webClient.DownloadStringCompleted += (s, e) =>
            {
                XDocument doc = XDocument.Parse(e.Result);
                if (doc.Root == null)
                {
                    throw new Exception("Could not get project build definitions");
                }
                XElement buildTypes = doc.Root.Element("buildTypes");
                if (buildTypes == null)
                {
                    throw new Exception("Could not get project build definitions");
                }
                TeamCityBuildDefinition[] projects = buildTypes
                    .Elements("buildType")
                    .Select(buildTypeXml => new TeamCityBuildDefinition(project.RootUrl, buildTypeXml))
                    .ToArray();
                complete(projects);
            };
            webClient.DownloadStringAsync(projectDetailsUrl);
        }

        private static bool _supportsGetLatestBuildByBuildTypeId = true;
        
        private static Dictionary<string, string> _cookies = new Dictionary<string, string>();

        private string GetCookie(string rootUrl)
        {
            string cookie;
            _cookies.TryGetValue(rootUrl, out cookie);
            return cookie;
        }

        public IList<TeamCityBuildStatus> GetBuildsStatuses(string rootUrl, string userName, string password, BuildDefinitionSetting[] watchedBuildDefinitions)
        {
            rootUrl = GetRootUrl(rootUrl);

            if (GetCookie(rootUrl) == null)
            {
                SetCookie(rootUrl, userName, password);
            }

            XDocument document = DownloadXml(rootUrl + "/ajax.html?getRunningBuilds=1", userName, password, GetCookie(rootUrl));

            var inProgressBuilds = document.Descendants("build").Select(b => new
            {
                buildTypeId = b.AttributeValueOrDefault("buildTypeId"),
                buildId = b.AttributeValueOrDefault("buildId")
            }).ToArray();

            var parallelResult = from buildDefinitionSetting in watchedBuildDefinitions
                                 from inProgressBuild in inProgressBuilds.Where(b => b.buildTypeId == buildDefinitionSetting.Id).DefaultIfEmpty()
                                 select inProgressBuild != null ?
                                    GetBuildStatusByBuildId(rootUrl, userName, password, buildDefinitionSetting, inProgressBuild.buildId)
                                    : GetBuildStatus(rootUrl, buildDefinitionSetting, userName, password);
            return parallelResult.AsParallel().ToList();
        }

        private void SetCookie(string rootUrl, string userName, string password)
        {
            int state = 0;
            DateTime initialRequest = DateTime.Now;
            bool serverUnavailable = false;
            Exception documentCompleteException = null;
            // WebBrowser needs to run in a single threaded apartment thread, see http://www.beansoftware.com/ASP.NET-Tutorials/Get-Web-Site-Thumbnail-Image.aspx
            Thread staThread = new Thread(() =>
            {
                // ajax.html is required to get in progress builds in TC 5.X, ajax.html requires forms auth, and TC encrypts
                //      its password on login.html, so WebBrowser is  required to get an authentication cookie
                using (WebBrowser webBrowser = new WebBrowser { Visible = true })
                {
                    webBrowser.DocumentCompleted += (o, evt) =>
                    {
                        try
                        {
                            if (webBrowser.Document == null) throw new SosException("WebBrowser.Document was null, this should never happen.");
                            _log.Debug("login.html State: " + state + " Cookie: " + webBrowser.Document.Cookie);

                            if (webBrowser.DocumentTitle == "Navigation Canceled")
                            {
                                serverUnavailable = true;
                                return;
                            }

                            if (state == 0)
                            {
                                HtmlElement usernameElement = webBrowser.Document.GetElementById("username");
                                if (usernameElement == null) throw new SosException("Expected an element with an id of 'username' but one didn't exist. Is TeamCity down?");
                                usernameElement.SetAttribute("value", userName);
                                usernameElement.SetAttribute("value", userName);
                                var htmlElement = webBrowser.Document.All["password"];
                                if (htmlElement != null)
                                    htmlElement.SetAttribute("value", password);

                                var submitButton = webBrowser.Document.GetElementsByTagName("input")
                                    .Cast<HtmlElement>()
                                    .FirstOrDefault(e => e.GetAttribute("type") == "submit");
                                if (submitButton != null) submitButton.InvokeMember("click");
                            }
                            if (state == 1)
                            {
                                _cookies[rootUrl] = webBrowser.Document.Cookie;
                            }

                            state++;
                        } catch (Exception ex)
                        {
                            documentCompleteException = ex;
                        }
                    };

                    string loginPage = rootUrl + "/login.html";
                    webBrowser.Navigate(new Uri(loginPage));

                    _log.Debug("Begin navigating to: " + loginPage);

                    while (state <= 1 && !serverUnavailable && documentCompleteException == null && !IsTimeout(initialRequest))
                    {
                        Application.DoEvents();
                    }
                    if (IsTimeout(initialRequest))
                    {
                        _log.Error("Timeout. State: " + state + " Cookie: " + GetCookie(rootUrl));
                    }
                        
                }
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            if (serverUnavailable)
                throw new ServerUnavailableException();
            if (documentCompleteException != null)
                throw documentCompleteException;
            if (IsTimeout(initialRequest))
            {
                throw new SosException("Timed out waiting for authentication, possible authentication error");
            }
        }

        private static bool IsTimeout(DateTime initialRequest)
        {
            const int timeoutSeconds = 30;
            return (DateTime.Now - initialRequest).TotalSeconds >= timeoutSeconds;
        }

        public TeamCityBuildStatus GetBuildStatus(string rootUrl, BuildDefinitionSetting buildDefinitionSetting, string userName, string password)
        {
            rootUrl = GetRootUrl(rootUrl);

            // older versions of team city don't support this format: httpAuth/app/rest/builds/buildType:bt2
            //  but that format requires far fewer http requests
            if (!_supportsGetLatestBuildByBuildTypeId)
            {
                // TeamCity 5.X
                return GetLatestBuildByBuildId(rootUrl, userName, password, buildDefinitionSetting);
            }
            // TeamCity 6.X
            return GetLatestBuildByBuildTypeId(rootUrl, userName, password, buildDefinitionSetting);
        }

        private static TeamCityBuildStatus GetLatestBuildByBuildId(string rootUrl, string userName, string password, BuildDefinitionSetting buildDefinitionSetting)
        {
            string getLatestBuildIdByBuildTypeUrl = rootUrl + "/httpAuth/app/rest/buildTypes/" + buildDefinitionSetting.Id + "/builds?count=1";
            try
            {
                XDocument latestBuildIdXDoc = DownloadXml(getLatestBuildIdByBuildTypeUrl, userName, password);
                var id = latestBuildIdXDoc.Descendants("build").Attributes("id").First().Value;
                return GetBuildStatusByBuildId(rootUrl, userName, password, buildDefinitionSetting, id);
            }
            catch (SosException ex)
            {
                if (ex.Message.Contains("No build type is found by id "))
                {
                    throw new BuildDefinitionNotFoundException(buildDefinitionSetting);
                }
                throw;
            }
        }

        private static TeamCityBuildStatus GetBuildStatusByBuildId(
            string rootUrl,
            string userName,
            string password,
            BuildDefinitionSetting buildDefinitionSetting,
            string buildId)
        {

            string getBuildByBuildIdIdUrl = rootUrl + "/httpAuth/app/rest/builds/id:" + buildId;
            XDocument buildResultXDoc = DownloadXml(getBuildByBuildIdIdUrl, userName, password);
            return GetBuildStatusAndCommentsFromXDocument(rootUrl, userName, password, buildDefinitionSetting, buildResultXDoc);
        }

        private static TeamCityBuildStatus GetBuildStatusAndCommentsFromXDocument(
            string rootUrl,
            string userName,
            string password,
            BuildDefinitionSetting buildDefinitionSetting,
            XDocument buildResultXDoc)
        {
            XDocument changeResultXDoc = null;
            if (buildResultXDoc.Root == null) throw new Exception("Could not get project build status");
            var changesNode = buildResultXDoc.Descendants("changes").First();
            var count = changesNode.AttributeValueOrDefault("count");
            bool commentsExist = !string.IsNullOrEmpty(count) && count != "0";
            if (commentsExist)
            {
                var changesHref = changesNode.AttributeValueOrDefault("href");
                var changesUrl = rootUrl + changesHref;
                XDocument changesResultXDoc = DownloadXml(changesUrl, userName, password);
                var changeNode = changesResultXDoc.Descendants("change").First();
                var changeHref = changeNode.AttributeValueOrDefault("href");
                var changeUrl = rootUrl + changeHref;
                changeResultXDoc = DownloadXml(changeUrl, userName, password);
            }
            return new TeamCityBuildStatus(buildDefinitionSetting, buildResultXDoc, changeResultXDoc);
        }

        private static TeamCityBuildStatus GetLatestBuildByBuildTypeId(string rootUrl, string userName, string password, BuildDefinitionSetting buildDefinitionSetting)
        {
            string url = rootUrl + "/httpAuth/app/rest/builds/buildType:" + buildDefinitionSetting.Id;
            try
            {
                XDocument doc = DownloadXml(url, userName, password);
                if (doc.Root == null) throw new Exception("Could not get project build status");
                return GetBuildStatusAndCommentsFromXDocument(rootUrl, userName, password, buildDefinitionSetting, doc);
            }
            catch (SosException ex)
            {
                if (ex.Message.Contains("BadRequestException: Cannot find build by other locator then 'id' without build type specified."))
                {
                    _log.Debug("_supportsGetLatestBuildByBuildTypeId = false");
                    _supportsGetLatestBuildByBuildTypeId = false;
                    return GetLatestBuildByBuildId(rootUrl, userName, password, buildDefinitionSetting);
                }
                if (ex.Message.Contains("No build type is found by id "))
                {
                    throw new BuildDefinitionNotFoundException(buildDefinitionSetting);
                }
                throw;
            }
        }

        private static XDocument DownloadXml(string url, string userName, string password, string cookie = null)
        {
            var webClient = new WebClient
            {
                Credentials = new NetworkCredential(userName, password),
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
            };

            if (cookie != null)
                webClient.Headers.Add("Cookie", cookie);

            try
            {
                var resultString = webClient.DownloadString(url);
                try
                {
                    return XDocument.Parse(resultString);
                }
                catch (Exception ex)
                {
                    string message = "Couldn't parse XML when trying to connect to " + url + ":\n" + resultString;
                    _log.Error(message, ex);
                    throw new SosException(message, ex);
                }
            }
            catch (WebException webException)
            {
                if (webException.Response != null)
                {
                    var response = webException.Response;
                    using (Stream s1 = response.GetResponseStream())
                    {
                        if (s1 != null)
                        {
                            using (StreamReader sr = new StreamReader(s1))
                            {
                                var errorResult = sr.ReadToEnd();
                                string message = "Error connecting to server with the following url: " + url + "\n\n" + errorResult;
                                _log.Error(message, webException);
                                throw new SosException(message, webException);
                            }
                        }
                    }
                }
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    throw new ServerUnavailableException();
                }
                throw;
            }
        }
    }
}
