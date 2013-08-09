using Shared.Utility;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;


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
			_originalTimeEntries = new ObservableCollection<ObservableTimeEntry>(projectTaskTimesheetItem.TimeEntries.Select(x => new ObservableTimeEntry(x)).ToList());
			

			// Set the properties to the _original property values
			ResetProperties();
			TimeEntries.CollectionChanged += TimeEntries_CollectionChanged;
			//TODO hook up  all of the property changed events for non-collection reference types
			foreach(var item in TimeEntries)
			{
				var propertyChangedItem = item as INotifyPropertyChanged;
				if(propertyChangedItem != null)
				{
					propertyChangedItem.PropertyChanged += TimeEntries_Item_PropertyChanged;
				}
			}
			

			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private void TimeEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_changeTracker["TimeEntries"] = true;
			

			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						var propertyChangedItem = item as INotifyPropertyChanged;
						if (propertyChangedItem != null)
						{
							propertyChangedItem.PropertyChanged -= TimeEntries_Item_PropertyChanged;
						}
						else
						{
							_changeTracker["TimeEntries"] = !ListUtilities<ObservableTimeEntry>.EqualTo(_originalTimeEntries,TimeEntries);
							OnPropertyChanged("IsChanged");
						}
					}
					break;
				case NotifyCollectionChangedAction.Add:
					foreach(var item in e.NewItems)
					{
						var propertyChangedItem = item as INotifyPropertyChanged;
						if (propertyChangedItem != null)
						{
							propertyChangedItem.PropertyChanged +=TimeEntries_Item_PropertyChanged;
						}
					}
					_changeTracker["TimeEntries"] = !ListUtilities<ObservableTimeEntry>.EqualTo(_originalTimeEntries,TimeEntries);
					OnPropertyChanged("IsChanged");
					break;
			}
		}
		

		private void TimeEntries_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var trackingItem = sender as IChangeTracking;
			if (trackingItem != null)
			{
				_changeTracker["TimeEntries"] = trackingItem.IsChanged;
				OnPropertyChanged("IsChanged");
			}
			else
			{
				_changeTracker["TimeEntries"] = !ListUtilities<ObservableTimeEntry>.EqualTo(_originalTimeEntries,TimeEntries);
				OnPropertyChanged("IsChanged");
			}
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
		

		private ObservableCollection<ObservableTimeEntry> _timeEntries;
		private ObservableCollection<ObservableTimeEntry> _originalTimeEntries;
		public ObservableCollection<ObservableTimeEntry> TimeEntries
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
			// Unhook propertyChanged eventhandlers for TimeEntries
			if (TimeEntries != null)
			{
				foreach(var item in TimeEntries)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged -= TimeEntries_Item_PropertyChanged;
					}
				}
			}
			TimeEntries = _originalTimeEntries == null ? null : GenericCopier<ObservableCollection<ObservableTimeEntry>>.DeepCopy(_originalTimeEntries);
			// Hook-up propertyChanged eventhandlers for TimeEntries
			if (TimeEntries != null)
			{
				foreach(var item in TimeEntries)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged += TimeEntries_Item_PropertyChanged;
					}
				}
			}
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
				return _changeTracker.Any(x => x.Value == true);
			}
			private set
			{
				throw new InvalidOperationException("Cannot set IsChanged property");
			}
		}
				
		#endregion
	}
}
