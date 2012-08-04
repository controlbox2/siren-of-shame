﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SirenOfShame.Lib.Settings;
using SirenOfShame.Lib.Watcher;

namespace SirenOfShame
{
    public partial class UserList : UserControl
    {
        public UserList()
        {
            InitializeComponent();
        }

        public event UserSelected OnUserSelected;
        public SirenOfShameSettings Settings { get; set; }

        private void InvokeOnUserSelected(string rawName)
        {
            var userSelected = OnUserSelected;
            if (userSelected != null)
            {
                userSelected(this, new UserSelectedArgs { RawName = rawName });
            }
        }

        private IEnumerable<UserPanel> GetUserPanels()
        {
            return _usersPanel.Controls.Cast<UserPanel>();
        }
        
        public void RefreshUserStats(IList<BuildStatus> changedBuildStatuses)
        {
            var userPanelsAndTheirPerson = from panel in GetUserPanels()
                                           join person in Settings.People on panel.RawName equals person.RawName
                                           select new {panel, person};
            userPanelsAndTheirPerson.ToList().ForEach(i => i.panel.RefreshStats(i.person));
        }

        public void Initialize(SirenOfShameSettings settings, ImageList avatarImageList)
        {
            Settings = settings;

            var peopleByReputation = settings.People.OrderBy(i => i.GetReputation());
            foreach (var person in peopleByReputation)
            {
                UserPanel userPanel = new UserPanel(person, avatarImageList)
                                          {
                                              Dock = DockStyle.Top,
                                              Cursor = Cursors.Hand,
                                              BackColor = Color.Transparent,
                                          };
                userPanel.MouseEnter += UserPanelMouseEnter;
                userPanel.Click += UserPanelClick;
                _usersPanel.Controls.Add(userPanel);
            }
        }

        void UserPanelClick(object sender, EventArgs e)
        {
            InvokeOnUserSelected(((UserPanel)sender).RawName);
        }

        void UserPanelMouseEnter(object sender, EventArgs e)
        {
            EnableMouseScrollWheel();
        }

        private void EnableMouseScrollWheel()
        {
            _usersPanel.Focus();
        }

        public void ChangeUserAvatarId(string rawName, int newImageIndex)
        {
            var userPanel = GetUserPanel(rawName);
            if (userPanel == null) return;
            userPanel.AvatarId = newImageIndex;
        }

        private UserPanel GetUserPanel(string rawName)
        {
            return GetUserPanels().FirstOrDefault(i => i.RawName == rawName);
        }
    }

    public delegate void UserSelected(object sender, UserSelectedArgs args);

    public class UserSelectedArgs
    {
        public string RawName { get; set; }
    }
}
