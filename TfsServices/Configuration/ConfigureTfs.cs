﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using log4net;
using SirenOfShame.Lib;
using SirenOfShame.Lib.ServerConfiguration;
using SirenOfShame.Lib.Settings;

namespace TfsServices.Configuration
{
    public partial class ConfigureTfs : ConfigureServerBase
    {
        private readonly TfsCiEntryPoint _tfsCiEntryPoint;
        private static readonly ILog _log = MyLogManager.GetLogger(typeof(ConfigureTfs));
        private readonly CiEntryPointSetting _ciEntryPointSetting;
        
        public ConfigureTfs(SirenOfShameSettings settings, TfsCiEntryPoint tfsCiEntryPoint, CiEntryPointSetting ciEntryPointSetting)
            : base(settings)
        {
            _tfsCiEntryPoint = tfsCiEntryPoint;
            InitializeComponent();
            _ciEntryPointSetting = ciEntryPointSetting;
            _url.Text = _ciEntryPointSetting.Url;
            DataBindAsync();
        }

        private void DataBindAsync()
        {
            if (_ciEntryPointSetting.Url == null)
            {
                // ToDo: dynamically try to find the url
                return;
            }

            _buildConfigurations.Nodes.Clear();
            _buildConfigurations.Nodes.Add("Contacting server...");

            Thread t = new Thread(DataBind) {IsBackground = true};
            t.Start();
        }

        private void DataBind()
        {
            try
            {
                using (var tfs = new MyTfsServer(_ciEntryPointSetting.Url))
                {
                    IList<TreeNode> projectCollectionNodes = new List<TreeNode>();

                    var myTfsProjectCollections = tfs.ProjectCollections.OrderBy(i => i.Name);
                    foreach (var teamProjectCollection in myTfsProjectCollections)
                    {
                        TreeNode projectCollectionNode = new TreeNode(teamProjectCollection.Name);
                        var myTfsProjects = teamProjectCollection.Projects.OrderBy(i => i.Name);
                        foreach (var project in myTfsProjects)
                        {
                            var projectNode = projectCollectionNode.Nodes.Add(project.Name);
                            var myTfsBuildDefinitions = project.BuildDefinitions.OrderBy(i => i.Name);
                            foreach (var buildDefinition in myTfsBuildDefinitions)
                            {
                                var buildDefinitionSetting = _ciEntryPointSetting.FindAddBuildDefinition(buildDefinition, _tfsCiEntryPoint.Name);
                                projectNode.Nodes.Add(buildDefinition.GetAsNode(buildDefinitionSetting.Active));
                            }
                        }
                        projectCollectionNodes.Add(projectCollectionNode);
                    }

                    Invoke(() => _buildConfigurations.Nodes.Clear());
                    Invoke(() => _buildConfigurations.Nodes.AddRange(projectCollectionNodes.ToArray()));

                }
                Settings.Save();
            }
            catch (Exception ex)
            {
                Invoke(() => _buildConfigurations.Nodes.Clear());
                _log.Error("Failed to connect to server", ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void GoClick(object sender, EventArgs e)
        {
            _ciEntryPointSetting.Url = _url.Text;
            Settings.Save();
            DataBindAsync();
        }

        private void BuildConfigurationsAfterCheck(object sender, TreeViewEventArgs e)
        {
            bool isParent = e.Node.Tag == null;
            if (isParent)
            {
                bool areAnyUnchecked = AreAnyUnchecked(e.Node);
                SetChecked(!areAnyUnchecked, e.Node);
                return;
            };
            var buildDefinitionId = (string)e.Node.Tag;
            _ciEntryPointSetting.GetBuildDefinition(buildDefinitionId).Active = e.Node.Checked;
            Settings.Save();
        }

        private void SetChecked(bool check, TreeNode parent)
        {
            parent.Checked = check;
            foreach (TreeNode child in parent.Nodes)
            {
                SetChecked(check, child);
            }
        }

        private bool AreAnyUnchecked(TreeNode parent)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (!child.Checked) return false;
                bool anyChildrenUnchecked = AreAnyUnchecked(child);
                if (anyChildrenUnchecked) return false;
            }
            return true;
        }

        private void UrlTextChanged(object sender, EventArgs e)
        {
            _ciEntryPointSetting.Url = _url.Text;
            Settings.Save();
        }
    }
}
