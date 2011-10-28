﻿using System;
using SirenOfShame.Lib.Settings;

namespace SirenOfShame.Configuration
{
    public partial class Settings : FormBase
    {
        private readonly SirenOfShameSettings _settings;

        public Settings(SirenOfShameSettings settings)
        {
            _settings = settings;
            InitializeComponent();

            _pollInterval.Value = _settings.PollInterval;
            RefreshDurationText();

            _updateLocationAuto.Checked = _settings.UpdateLocation == UpdateLocation.Auto;
            _updateLocationOther.Checked = _settings.UpdateLocation == UpdateLocation.Other;
            _updateLocationOtherLocation.Text = _settings.UpdateLocation == UpdateLocation.Other ? _settings.UpdateLocationOther : "";

            _viewLog.Enabled = Program.Form.CanViewLogs;
        }

        private void OkClick(object sender, EventArgs e)
        {
            _settings.PollInterval = _pollInterval.Value;
            UpdateLocation updateLocation;
            _settings.UpdateLocationOther = null;
            if (_updateLocationAuto.Checked)
            {
                updateLocation = UpdateLocation.Auto;
            }
            else if (_updateLocationOther.Checked)
            {
                updateLocation = UpdateLocation.Other;
                _settings.UpdateLocationOther = _updateLocationOtherLocation.Text;
            }
            else
            {
                throw new NotImplementedException("One of the update locations needs to be checked");
            }
            _settings.UpdateLocation = updateLocation;
            _settings.Save();
            Close();
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

        private void _updateLocationOther_CheckedChanged(object sender, EventArgs e)
        {
            _updateLocationOtherLocation.Enabled = _updateLocationOther.Checked;
        }

        private void _checkForUpdates_Click(object sender, EventArgs e)
        {
            Close();
            Program.Form.CheckForUpdates();
        }
    }
}
