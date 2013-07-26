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
			
		}
		
		public bool IsChanged
		{
			get 
		    { 
				throw new NotImplementedException(); 
			}
		}
		        
		#endregion
	}
}
