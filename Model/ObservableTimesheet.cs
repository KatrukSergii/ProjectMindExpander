using Model;
using Shared;
using Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Shared.DataStructures;


namespace Model
{
	[Serializable]
	public partial class ObservableTimesheet : INotifyPropertyChanged, IChangeTracking
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservableTimesheet()
		{
			InitializeChangeTracker();
			Title = default(string);
			TimesheetId = default(string);
			ProjectTimeItems = null;
			NonProjectActivityItems = null;
			RequiredHours = null;
			TotalRequiredHours = new TimeSpan();
			DummyTimeEntry = new ObservableTimeEntry();
		}
		

		public ObservableTimesheet(Timesheet timesheet) : this()
		{
			_originalTitle = timesheet.Title;
			_originalTimesheetId = timesheet.TimesheetId;
			_originalProjectTimeItems = new ObservableList<ObservableProjectTaskTimesheetItem>(timesheet.ProjectTimeItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalNonProjectActivityItems = new ObservableList<ObservableProjectTaskTimesheetItem>(timesheet.NonProjectActivityItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalRequiredHours = new ObservableList<TimeSpan>(timesheet.RequiredHours);
			_originalTotalRequiredHours = timesheet.TotalRequiredHours;
			_originalDummyTimeEntry = new ObservableTimeEntry(timesheet.DummyTimeEntry);
			

			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
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
					if (_originalTitle == null || !_originalTitle.Equals(_title))
					{
						_changeTracker["Title"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Title"] = false;
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
					if (_originalTimesheetId == null || !_originalTimesheetId.Equals(_timesheetId))
					{
						_changeTracker["TimesheetId"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TimesheetId"] = false;
					}
				}
			}
		}
		

		private ObservableList<ObservableProjectTaskTimesheetItem> _projectTimeItems;
		private ObservableList<ObservableProjectTaskTimesheetItem> _originalProjectTimeItems;
		public ObservableList<ObservableProjectTaskTimesheetItem> ProjectTimeItems
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
					if (_originalProjectTimeItems == null || !_originalProjectTimeItems.Equals(_projectTimeItems))
					{
						_changeTracker["ProjectTimeItems"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ProjectTimeItems"] = false;
					}
				}
			}
		}
		

		private ObservableList<ObservableProjectTaskTimesheetItem> _nonProjectActivityItems;
		private ObservableList<ObservableProjectTaskTimesheetItem> _originalNonProjectActivityItems;
		public ObservableList<ObservableProjectTaskTimesheetItem> NonProjectActivityItems
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
					if (_originalNonProjectActivityItems == null || !_originalNonProjectActivityItems.Equals(_nonProjectActivityItems))
					{
						_changeTracker["NonProjectActivityItems"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["NonProjectActivityItems"] = false;
					}
				}
			}
		}
		

		private ObservableList<TimeSpan> _requiredHours;
		private ObservableList<TimeSpan> _originalRequiredHours;
		public ObservableList<TimeSpan> RequiredHours
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
					if (_originalRequiredHours == null || !_originalRequiredHours.Equals(_requiredHours))
					{
						_changeTracker["RequiredHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["RequiredHours"] = false;
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
					if (_originalTotalRequiredHours == null || !_originalTotalRequiredHours.Equals(_totalRequiredHours))
					{
						_changeTracker["TotalRequiredHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TotalRequiredHours"] = false;
					}
				}
			}
		}
		

		private ObservableTimeEntry _dummyTimeEntry;
		private ObservableTimeEntry _originalDummyTimeEntry;
		public ObservableTimeEntry DummyTimeEntry
		{
			get
			{
				return _dummyTimeEntry;
			}
			set
			{
				if (_dummyTimeEntry != value)
				{
					_dummyTimeEntry = value;
					OnPropertyChanged("DummyTimeEntry");
					if (_originalDummyTimeEntry == null || !_originalDummyTimeEntry.Equals(_dummyTimeEntry))
					{
						_changeTracker["DummyTimeEntry"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["DummyTimeEntry"] = false;
					}
				}
			}
		}
		

		private void ResetProperties()
		{
			Title = _originalTitle;
			TimesheetId = _originalTimesheetId;
			ProjectTimeItems = _originalProjectTimeItems == null ? null : _originalProjectTimeItems.DeepCopy();
			NonProjectActivityItems = _originalNonProjectActivityItems == null ? null : _originalNonProjectActivityItems.DeepCopy();
			RequiredHours = _originalRequiredHours == null ? null : _originalRequiredHours.DeepCopy();
			TotalRequiredHours = _originalTotalRequiredHours;
			DummyTimeEntry = _originalDummyTimeEntry == null ? null : GenericCopier<ObservableTimeEntry>.DeepCopy(_originalDummyTimeEntry);
		}
		

		
		#region INotifyPropertyChanged
		
		[field:NonSerializedAttribute()]
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (_isTrackingEnabled)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
			}
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
			_originalDummyTimeEntry = _dummyTimeEntry;
			ResetChangeTracking();
		}
		

		public void AbandonChanges()
		{
			_isTrackingEnabled = false;
			ResetProperties();
			_isTrackingEnabled = true;
			ResetChangeTracking();
		}
		

		private void InitializeChangeTracker()
		{
			_changeTracker = new Dictionary<string,bool>();
			_changeTracker["Title"] = false;
			_changeTracker["TimesheetId"] = false;
			_changeTracker["ProjectTimeItems"] = false;
			_changeTracker["NonProjectActivityItems"] = false;
			_changeTracker["RequiredHours"] = false;
			_changeTracker["TotalRequiredHours"] = false;
			_changeTracker["DummyTimeEntry"] = false;
		}
		
		
		private void ResetChangeTracking()
		{
			foreach (string key in _changeTracker.Keys.ToList())
			{
				_changeTracker[key] = false;
			}
		}
		
		public bool IsChanged
		{
			get 
			{ 
				return _changeTracker.All(x => x.Value == false);
			}
			private set
			{
				throw new InvalidOperationException("Cannot set IsChanged property");
			}
		}
				
		#endregion
	}
}
