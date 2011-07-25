﻿using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Framework.Client;
using SirenOfShame.Lib;
using SirenOfShame.Lib.Exceptions;
using SirenOfShame.Lib.Settings;
using SirenOfShame.Lib.Watcher;
using TfsServices.Configuration;
using BuildStatus = SirenOfShame.Lib.Watcher.BuildStatus;

namespace TfsServices
{
    public class TfsWatcher : WatcherBase
    {
        private static readonly ILog Log = MyLogManager.GetLogger(typeof(TfsWatcher));
        private readonly TfsCiEntryPoint _tfsCiEntryPoint;

        public TfsWatcher(SirenOfShameSettings settings, TfsCiEntryPoint tfsCiEntryPoint) : base(settings)
        {
            _tfsCiEntryPoint = tfsCiEntryPoint;
        }

        private MyTfsServer _myTfsServer;
        private MyTfsBuildDefinition[] _watchedBuildDefinitions;

        protected override IEnumerable<BuildStatus> GetBuildStatus()
        {
            try {
                if (_myTfsServer == null) _myTfsServer = new MyTfsServer(Settings.FindAddSettings(_tfsCiEntryPoint.Name).Url);
                if (_watchedBuildDefinitions == null)
                {
                    _watchedBuildDefinitions = GetAllWatchedBuildDefinitions().ToArray();
                }

                var buildDefinitionsByServer = _watchedBuildDefinitions.GroupBy(bd => bd.BuildServer);
                return buildDefinitionsByServer.SelectMany(g => g.Key.GetBuildStatuses(g));
            }
            catch (DatabaseConnectionException ex)
            {
                throw new ServerUnavailableException(ex.Message, ex);
            }
            catch (TeamFoundationServiceUnavailableException ex)
            {
                throw new ServerUnavailableException(ex.Message, ex);
            }
            catch (BuildServerException ex)
            {
                Log.Error("Logging a BuildServerException as Server Unavailable Exception so SoS will continue to try to find the server", ex);
                throw new ServerUnavailableException(ex.Message, ex);
            }
        }

        private IEnumerable<MyTfsBuildDefinition> GetAllWatchedBuildDefinitions()
        {
            IEnumerable<MyTfsBuildDefinition> allTfsBuildDefinitions = _myTfsServer.ProjectCollections
                .SelectMany(pc => pc.Projects)
                .SelectMany(p => p.BuildDefinitions);

            var activeBuildDefinitionSettings = Settings.BuildDefinitionSettings.Where(bd => bd.Active && bd.BuildServer == _tfsCiEntryPoint.Name && bd.BuildServer == Settings.ServerType);
            return from buildDef in allTfsBuildDefinitions
                   join buildDefSetting in activeBuildDefinitionSettings on
                       buildDef.Id equals buildDefSetting.Id
                   select buildDef;
        }

        private void Reset()
        {
            if (_myTfsServer != null)
            {
                _myTfsServer.Dispose();
                _myTfsServer = null;
            }
            _watchedBuildDefinitions = null;
        }

        public override void StopWatching()
        {
            Reset();
        }

        public override void Dispose()
        {
            Reset();
        }
    }
}
