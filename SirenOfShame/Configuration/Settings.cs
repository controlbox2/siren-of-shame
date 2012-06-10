﻿using System;
using SirenOfShame.Lib.Settings;

namespace SirenOfShame.Configuration
{
    public partial class Settings : FormBase
    {
        private readonly SirenOfShameSettings _settings;
        private bool _resetReputationOnSave = false;

        public Settings(SirenOfShameSettings settings)
        {
            _settings = settings;
            InitializeComponent();

            _pollInterval.Value = _settings.PollInterval;
            RefreshDurationText();

            _updateLocationAuto.Checked = _settings.UpdateLocation == UpdateLocation.Auto;
            _updateLocationOther.Checked = _settings.UpdateLocation == UpdateLocation.Other;
            _updateLocationNever.Checked = _settings.UpdateLocation == UpdateLocation.Never;
            _updateLocationOtherLocation.Text = _settings.UpdateLocation == UpdateLocation.Other ? _settings.UpdateLocationOther : "";
            _hideReputation.Checked = _settings.HideReputation;

            _viewLog.Enabled = Program.Form.CanViewLogs;

            InitializeAchievementAlertPreferences();
            InitializeUserIAm();
        }

        private void InitializeUserIAm()
        {
            _userIAm.Items.Add("");
            foreach (var personInProject in _settings.People)
            {
                _userIAm.Items.Add(personInProject);
            }
            if (!string.IsNullOrEmpty(_settings.MyRawName))
            {
                foreach (var item in _userIAm.Items)
                {
                    var personSetting = item as PersonSetting;
                    if (personSetting != null && personSetting.RawName == _settings.MyRawName)
                    {
                        _userIAm.SelectedItem = item;
                    }
                }
            }
        }

        private void InitializeAchievementAlertPreferences()
        {
            if (_settings.AchievementAlertPreference == AchievementAlertPreferenceEnum.Always)
            {
                _alwaysShowNewAchievements.Checked = true;
            }
            if (_settings.AchievementAlertPreference == AchievementAlertPreferenceEnum.Never)
            {
                _neverShowAchievements.Checked = true;
            }
            if (_settings.AchievementAlertPreference == AchievementAlertPreferenceEnum.OnlyForMe)
            {
                _onlyShowMyAchievements.Checked = true;
            }
        }

        private UpdateLocation GetUpdateLocation()
        {
            if (_updateLocationAuto.Checked)
            {
                return UpdateLocation.Auto;
            }
            if (_updateLocationOther.Checked)
            {
                return UpdateLocation.Other;
            }
            if (_updateLocationNever.Checked)
            {
                return UpdateLocation.Never;
            }
            throw new NotImplementedException("One of the update locations needs to be checked");
        }

        private void OkClick(object sender, EventArgs e)
        {
            _settings.PollInterval = _pollInterval.Value;
            UpdateLocation updateLocation = GetUpdateLocation();
            _settings.UpdateLocationOther = null;
            _settings.UpdateLocationOther = _updateLocationOtherLocation.Text;
            _settings.UpdateLocation = updateLocation;

            if (_resetReputationOnSave)
            {
                _settings.People.ForEach(p =>
                {
                    p.TotalBuilds = 0;
                    p.FailedBuilds = 0;
                });
            }

            _settings.HideReputation = _hideReputation.Checked;

            SetShowAchievements();
            SetUserIAm();

            _settings.Save();
            Close();
        }

        private void SetUserIAm()
        {
            if (_userIAm.SelectedItem as string == "")
            {
                _settings.MyRawName = null;
            } else
            {
                _settings.MyRawName = ((PersonSetting) _userIAm.SelectedItem).RawName;
            }
        }

        private void SetShowAchievements()
        {
            if (_alwaysShowNewAchievements.Checked)
            {
                _settings.AchievementAlertPreference = AchievementAlertPreferenceEnum.Always;
            }
            if (_neverShowAchievements.Checked)
            {
                _settings.AchievementAlertPreference = AchievementAlertPreferenceEnum.Never;
            }
            if (_onlyShowMyAchievements.Checked)
            {
                _settings.AchievementAlertPreference = AchievementAlertPreferenceEnum.OnlyForMe;
            }
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PollIntervalValueChanged(object sender, EventArgs e)
        {
            RefreshDurationText();
        }

        private void RefreshDurationText()
        {
            string snideComment = ".";
            if (_pollInterval.Value <= 5)
                snideComment = ". (yes, I hate my sys admin that much)";
            if (_pollInterval.Value <= 1)
                snideComment = ". (yes, I really hate my co-workers that much, but s'alright, will blame the authors of this app)";
            _duration.Text = string.Format("{0} seconds{1}", _pollInterval.Value, snideComment);
        }

        private void ViewLogClick(object sender, EventArgs e)
        {
            Program.Form.ViewLogs();
        }

        private void UpdateLocationOtherCheckedChanged(object sender, EventArgs e)
        {
            _updateLocationOtherLocation.Enabled = _updateLocationOther.Checked;
        }

        private void CheckForUpdatesClick(object sender, EventArgs e)
        {
            Program.Form.CheckForUpdates();
        }

        private void ResetReputationClick(object sender, EventArgs e)
        {
            _resetReputationOnSave = true;
        }

        private void RecalculateClick(object sender, EventArgs e)
        {
            FindOldAchievements.TryFindOldAchievements(_settings);
        }
    }
}
