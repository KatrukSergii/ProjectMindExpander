using Model;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Model
{
	public class TrackableTimesheet : INotifyPropertyChanged, IChangeTracking
	{
		public TrackableTimesheet()
		{
			Title = default(string);
			TimesheetId = default(string);
			ProjectTimeItems = new List<ProjectTaskTimesheetItem>();
			NonProjectActivityItems = new List<ProjectTaskTimesheetItem>();
			RequiredHours = new List<TimeSpan>();
			TotalRequiredHours = new TimeSpan();
		}
		

		public TrackableTimesheet(Timesheet timesheet)
		{
			_originalTitle = timesheet.Title;
			_originalTimesheetId = timesheet.TimesheetId;
			_originalProjectTimeItems = timesheet.ProjectTimeItems;
			_originalNonProjectActivityItems = timesheet.NonProjectActivityItems;
			_originalRequiredHours = timesheet.RequiredHours;
			_originalTotalRequiredHours = timesheet.TotalRequiredHours;
		}
		

		private string _title;
		private string _originalTitle;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (_title != value)
				{
					_title = value;
					OnPropertyChanged("Title");
					if (_originalTitle != _title)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private string _timesheetId;
		private string _originalTimesheetId;
		public string TimesheetId
		{
			get
			{
				return _timesheetId;
			}
			set
			{
				if (_timesheetId != value)
				{
					_timesheetId = value;
					OnPropertyChanged("TimesheetId");
					if (_originalTimesheetId != _timesheetId)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private List<ProjectTaskTimesheetItem> _projectTimeItems;
		private List<ProjectTaskTimesheetItem> _originalProjectTimeItems;
		public List<ProjectTaskTimesheetItem> ProjectTimeItems
		{
			get
			{
				return _projectTimeItems;
			}
			set
			{
				if (_projectTimeItems != value)
				{
					_projectTimeItems = value;
					OnPropertyChanged("ProjectTimeItems");
					if (_originalProjectTimeItems != _projectTimeItems)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private List<ProjectTaskTimesheetItem> _nonProjectActivityItems;
		private List<ProjectTaskTimesheetItem> _originalNonProjectActivityItems;
		public List<ProjectTaskTimesheetItem> NonProjectActivityItems
		{
			get
			{
				return _nonProjectActivityItems;
			}
			set
			{
				if (_nonProjectActivityItems != value)
				{
					_nonProjectActivityItems = value;
					OnPropertyChanged("NonProjectActivityItems");
					if (_originalNonProjectActivityItems != _nonProjectActivityItems)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private List<TimeSpan> _requiredHours;
		private List<TimeSpan> _originalRequiredHours;
		public List<TimeSpan> RequiredHours
		{
			get
			{
				return _requiredHours;
			}
			set
			{
				if (_requiredHours != value)
				{
					_requiredHours = value;
					OnPropertyChanged("RequiredHours");
					if (_originalRequiredHours != _requiredHours)
					{
						IsChanged = true;
					}
				}
			}
		}
		

		private TimeSpan _totalRequiredHours;
		private TimeSpan _originalTotalRequiredHours;
		public TimeSpan TotalRequiredHours
		{
			get
			{
				return _totalRequiredHours;
			}
			set
			{
				if (_totalRequiredHours != value)
				{
					_totalRequiredHours = value;
					OnPropertyChanged("TotalRequiredHours");
					if (_originalTotalRequiredHours != _totalRequiredHours)
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
			_originalTitle = _title;
			_originalTimesheetId = _timesheetId;
			_originalProjectTimeItems = _projectTimeItems;
			_originalNonProjectActivityItems = _nonProjectActivityItems;
			_originalRequiredHours = _requiredHours;
			_originalTotalRequiredHours = _totalRequiredHours;
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
