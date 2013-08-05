using Model;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Model
{
	public class TrackableProjectTaskTimesheetItem : INotifyPropertyChanged, IChangeTracking
	{
		public TrackableProjectTaskTimesheetItem()
		{
			ProjectCode = new PickListItem();
			TaskCode = new PickListItem();
			TimeEntries = new List<TimeEntry>();
		}
		

		public TrackableProjectTaskTimesheetItem(ProjectTaskTimesheetItem projectTaskTimesheetItem)
		{
			_originalProjectCode = projectTaskTimesheetItem.ProjectCode;
			_originalTaskCode = projectTaskTimesheetItem.TaskCode;
			_originalTimeEntries = projectTaskTimesheetItem.TimeEntries;
		}
		

		private PickListItem _projectCode;
		private PickListItem _originalProjectCode;
		public PickListItem ProjectCode
		{
			get
			{
				return _projectCode;
			}
			set
			{
				if (_projectCode != value)
				{
					_projectCode = value;
					OnPropertyChanged("ProjectCode");
					if (_originalProjectCode != _projectCode)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private PickListItem _taskCode;
		private PickListItem _originalTaskCode;
		public PickListItem TaskCode
		{
			get
			{
				return _taskCode;
			}
			set
			{
				if (_taskCode != value)
				{
					_taskCode = value;
					OnPropertyChanged("TaskCode");
					if (_originalTaskCode != _taskCode)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private List<TimeEntry> _timeEntries;
		private List<TimeEntry> _originalTimeEntries;
		public List<TimeEntry> TimeEntries
		{
			get
			{
				return _timeEntries;
			}
			set
			{
				if (_timeEntries != value)
				{
					_timeEntries = value;
					OnPropertyChanged("TimeEntries");
					if (_originalTimeEntries != _timeEntries)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		
		#endregion

		#region IChangeTracking

		public void AcceptChanges()
		{
			_originalProjectCode = _projectCode;
			_originalTaskCode = _taskCode;
			_originalTimeEntries = _timeEntries;
			IsChanged = false;
		}
		
		
		private bool _isChanged;
		public bool IsChanged
		{
			get 
			{ 
				return _isChanged;
			}
			set
			{
				if (_isChanged != value)
				{
					_isChanged = value;
					OnPropertyChanged("IsChanged");
				}
			}
		}
				
		#endregion
	}
}
