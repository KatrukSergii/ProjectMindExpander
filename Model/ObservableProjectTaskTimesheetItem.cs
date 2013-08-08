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
	public partial class ObservableProjectTaskTimesheetItem : INotifyPropertyChanged, IChangeTracking
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservableProjectTaskTimesheetItem()
		{
			InitializeChangeTracker();
			ProjectCode = new PickListItem();
			TaskCode = new PickListItem();
			TimeEntries = null;
		}
		

		public ObservableProjectTaskTimesheetItem(ProjectTaskTimesheetItem projectTaskTimesheetItem) : this()
		{
			_originalProjectCode = projectTaskTimesheetItem.ProjectCode;
			_originalTaskCode = projectTaskTimesheetItem.TaskCode;
			_originalTimeEntries = new ObservableList<ObservableTimeEntry>(projectTaskTimesheetItem.TimeEntries.Select(x => new ObservableTimeEntry(x)).ToList());
			

			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
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
					if (_originalProjectCode == null || !_originalProjectCode.Equals(_projectCode))
					{
						_changeTracker["ProjectCode"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ProjectCode"] = false;
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
					if (_originalTaskCode == null || !_originalTaskCode.Equals(_taskCode))
					{
						_changeTracker["TaskCode"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TaskCode"] = false;
					}
				}
			}
		}
		

		private ObservableList<ObservableTimeEntry> _timeEntries;
		private ObservableList<ObservableTimeEntry> _originalTimeEntries;
		public ObservableList<ObservableTimeEntry> TimeEntries
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
					if (_originalTimeEntries == null || !_originalTimeEntries.Equals(_timeEntries))
					{
						_changeTracker["TimeEntries"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TimeEntries"] = false;
					}
				}
			}
		}
		

		private void ResetProperties()
		{
			ProjectCode = _originalProjectCode == null ? null : GenericCopier<PickListItem>.DeepCopy(_originalProjectCode);
			TaskCode = _originalTaskCode == null ? null : GenericCopier<PickListItem>.DeepCopy(_originalTaskCode);
			TimeEntries = _originalTimeEntries == null ? null : _originalTimeEntries.DeepCopy();
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
			_originalProjectCode = _projectCode;
			_originalTaskCode = _taskCode;
			_originalTimeEntries = _timeEntries;
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
			_changeTracker["ProjectCode"] = false;
			_changeTracker["TaskCode"] = false;
			_changeTracker["TimeEntries"] = false;
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
