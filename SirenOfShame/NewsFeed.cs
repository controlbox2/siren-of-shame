﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace SirenOfShame
{
    public partial class NewsFeed : UserControl
    {
        public NewsFeed()
        {
            InitializeComponent();
            //newsItemHeightAnimator.Interval = 10;
            //newsItemHeightAnimator.Tick += NewsItemHeightAnimatorOnTick;
        }

        //private List<NewsItem> newsItemsToOpen = new List<NewsItem>();
        
        //private void NewsItemHeightAnimatorOnTick(object sender, EventArgs eventArgs)
        //{
            
        //}

        private int _newsItemCount = 0;

        //Timer newsItemHeightAnimator = new Timer();

        private void Button1Click(object sender, EventArgs e)
        {
            string userName = "Joe Ferner " + _newsItemCount;
            const string description = " broke the build with a comment of \"Fixing Lee's bunk check-in from yesterday\"";
            var newsItem = new NewsItem(userName, description) {Dock = DockStyle.Top};
            Controls.Add(newsItem);
            newsItem.Height = newsItem.GetIdealHeight();
            _newsItemCount++;
        }

        private void NewsFeedResize(object sender, EventArgs e)
        {
            Controls
                .Cast<Control>()
                .Select(i => i as NewsItem)
                .Where(i => i != null)
                .ToList()
                .ForEach(i => i.Height = i.GetIdealHeight());
        }
    }
}
