﻿using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SirenOfShame.Lib;
using SirenOfShame.Lib.Helpers;
using SirenOfShame.Lib.Settings;
using SirenOfShame.Lib.Watcher;
using log4net;

namespace TeamCityServices
{
    /*
<build id="2" number="2" status="SUCCESS" href="/httpAuth/app/rest/builds/id:2" webUrl="http://vmxp:8080/viewLog.html?buildId=2&buildTypeId=bt2" personal="false" history="false" pinned="false">
    <statusText>Success</statusText>
    <buildType id="bt2" name="Test Project 1 Build" href="/httpAuth/app/rest/buildTypes/id:bt2" projectName="Test Project 1" projectId="project2" webUrl="http://vmxp:8080/viewType.html?buildTypeId=bt2"/>
    <startDate>20110606T040133+0300</startDate>
    <finishDate>20110606T040142+0300</finishDate>
    <agent name="vmxp" id="1" href="/httpAuth/app/rest/agents/id:1"/>
    <tags/>
    <properties/>
    <revisions>
        <revision display-version="0f76111b776a">
            <vcs-root href="/httpAuth/app/rest/vcs-roots/id:1" name="mercurial: C:\hgrepo\TestProject1"/>
        </revision>
    </revisions>
    <changes href="/httpAuth/app/rest/changes?build=id:2" count="0"/>
    <relatedIssues/>
</build>
     */
    public class TeamCityBuildStatus
    {
        private static readonly ILog _log = MyLogManager.GetLogger(typeof(TeamCityBuildStatus));

        public string BuildDefinitionId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime StartedTime { get; set; }
        public DateTime FinishedTime { get; set; }
        public BuildStatusEnum BuildStatus { get; set; }
        public string BuildDefinitionName { get; set; }
        public string Comment { get; set; }

        // todo: parse this better
        private static DateTime GetTeamCityDate(string startedTimeStr)
        {
            startedTimeStr = startedTimeStr.Replace('T', ' ');
            int i = startedTimeStr.IndexOf('+');
            if (i > 0)
            {
                startedTimeStr = startedTimeStr.Substring(0, i);
            }
            i = startedTimeStr.IndexOf('-');
            if (i > 0)
            {
                startedTimeStr = startedTimeStr.Substring(0, i);
            }
            return DateTime.ParseExact(startedTimeStr, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);
        }

        public TeamCityBuildStatus(BuildDefinitionSetting definition, XDocument doc)
        {
            string status = doc.Root.AttributeValueOrDefault("status");
            string startedTimeStr = doc.Root.ElementValueOrDefault("startDate");
            string finishedTimeStr = doc.Root.ElementValueOrDefault("finishDate");

            BuildDefinitionId = definition.Id;
            BuildDefinitionName = definition.Name;
            StartedTime = GetTeamCityDate(startedTimeStr);
            if (string.IsNullOrEmpty(finishedTimeStr))
            {
                BuildStatus = BuildStatusEnum.InProgress;
            } else
            {
                FinishedTime = GetTeamCityDate(finishedTimeStr);
                BuildStatus = ToBuildStatusEnum(status);
            }
        }

        public TeamCityBuildStatus(BuildDefinitionSetting buildDefinitionSetting, XDocument buildResultXDoc, XDocument changeResultXDoc)
            : this(buildDefinitionSetting, buildResultXDoc)
        {
            Comment = changeResultXDoc.Descendants("comment").First().Value;
            RequestedBy = changeResultXDoc.Root.AttributeValueOrDefault("username");
        }

        private BuildStatusEnum ToBuildStatusEnum(string status)
        {
            switch (status)
            {
                case "SUCCESS":
                    return BuildStatusEnum.Working;
                case "FAILURE":
                    return BuildStatusEnum.Broken;
                default:
                    _log.Warn("Unknown build status: " + status);
                    return BuildStatusEnum.Unknown;
            }
        }

        public BuildStatus ToBuildStatus()
        {
            return new BuildStatus
            {
                BuildStatusEnum = BuildStatus,
                FinishedTime = FinishedTime,
                Comment = Comment,
                Id = BuildDefinitionId,
                Name = BuildDefinitionName,
                RequestedBy = RequestedBy,
                StartedTime = StartedTime
            };
        }
    }
}