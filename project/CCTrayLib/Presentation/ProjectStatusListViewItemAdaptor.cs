using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStatusListViewItemAdaptor
	{
		private readonly IDetailStringProvider detailStringProvider;
		private readonly ListViewItem item = new ListViewItem();
		private readonly ListViewItem.ListViewSubItem activity;
		private readonly ListViewItem.ListViewSubItem detail;
		private readonly ListViewItem.ListViewSubItem lastBuildLabel;
		private readonly ListViewItem.ListViewSubItem lastBuildTime;
		private readonly ListViewItem.ListViewSubItem serverName;
		private readonly ICCTrayMultiConfiguration config = null;
		
		public ProjectStatusListViewItemAdaptor(IDetailStringProvider detailStringProvider, ICCTrayMultiConfiguration config) : this(detailStringProvider)
		{
			this.config = config;	
		}

		public ProjectStatusListViewItemAdaptor(IDetailStringProvider detailStringProvider)
		{
			this.detailStringProvider = detailStringProvider;
			serverName = new ListViewItem.ListViewSubItem();
			item.SubItems.Add(serverName);
			activity = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(activity);
			detail = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(detail);
			lastBuildLabel = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(lastBuildLabel);
			lastBuildTime = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(lastBuildTime);
		}

		public ListViewItem Create(IProjectMonitor projectMonitor)
		{
			projectMonitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);

			item.Text = projectMonitor.Detail.ProjectName;

			DisplayProjectStateInListViewItem(projectMonitor);

			return item;
		}

		private void Monitor_Polled(object sauce, MonitorPolledEventArgs args)
		{
			DisplayProjectStateInListViewItem(args.ProjectMonitor);
		}

		private void DisplayProjectStateInListViewItem(IProjectMonitor monitor)
		{
			item.ImageIndex = monitor.ProjectState.ImageIndex;

			if (monitor.Detail.IsConnected)
			{
				if (config != null)
				{
					foreach(CCTrayProject project in config.Projects)
					{
						if(project.ProjectName == monitor.Detail.ProjectName)
						{
							serverName.Text = project.BuildServer.DisplayName;
						}
					}
				}
				else
				{
					serverName.Text = new Uri(monitor.Detail.WebURL).Host;
				}
				lastBuildLabel.Text = monitor.Detail.LastBuildLabel;
				lastBuildTime.Text = monitor.Detail.LastBuildTime.ToString();
				activity.Text = monitor.Detail.Activity.ToString();
			}
			else
			{
				activity.Text = lastBuildLabel.Text = "";
			}

			detail.Text = detailStringProvider.FormatDetailString(monitor.Detail);
		}
	}
}
