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
			ProjectCode = new ObservablePickListItem();
			TaskCode = new ObservablePickListItem();
			TimeEntries = null;
		}
		public ObservableProjectTaskTimesheetItem(ProjectTaskTimesheetItem projectTaskTimesheetItem) : this()
		{
			_originalProjectCode = new ObservablePickListItem(projectTaskTimesheetItem.ProjectCode);
			_originalTaskCode = new ObservablePickListItem(projectTaskTimesheetItem.TaskCode);
			_originalTimeEntries = new ObservableCollection<ObservableTimeEntry>(projectTaskTimesheetItem.TimeEntries.Select(x => new ObservableTimeEntry(x)).ToList());
			

			// Set the properties to the _original property values
			ResetProperties();
			TimeEntries.CollectionChanged += TimeEntries_CollectionChanged;
			foreach(var item in TimeEntries)
			{
				var propertyChangedItem = item as INotifyPropertyChanged;
				if(propertyChangedItem != null)
				{
					propertyChangedItem.PropertyChanged += TimeEntries_Item_PropertyChanged;
				}
			}
			

			ProjectCode.PropertyChanged += ProjectCode_PropertyChanged;
			TaskCode.PropertyChanged += TaskCode_PropertyChanged;
			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private void TimeEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			

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
							// always treat a remove or replace on a reference type as a change
							_changeTracker["ProjectTimeItems"] = true;
						}
						else
						{
							_changeTracker["TimeEntries"] = !ListUtility.EqualTo(_originalTimeEntries,TimeEntries);
						}
						OnPropertyChanged("IsChanged");
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
					_changeTracker["TimeEntries"] = !ListUtility.EqualTo(_originalTimeEntries,TimeEntries);
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
				_changeTracker["TimeEntries"] = !ListUtility.EqualTo(_originalTimeEntries,TimeEntries);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private ObservablePickListItem _projectCode;
		private ObservablePickListItem _originalProjectCode;
		public ObservablePickListItem ProjectCode
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
		

		private ObservablePickListItem _taskCode;
		private ObservablePickListItem _originalTaskCode;
		public ObservablePickListItem TaskCode
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
			// Unhook propertyChanged eventhandlers for ProjectCode
			if (ProjectCode != null) ProjectCode.PropertyChanged -= ProjectCode_PropertyChanged;
			ProjectCode = _originalProjectCode == null ? null : GenericCopier<ObservablePickListItem>.DeepCopy(_originalProjectCode);
			// Hookup propertyChanged eventhandlers for ProjectCode
			if (ProjectCode != null) ProjectCode.PropertyChanged += ProjectCode_PropertyChanged;
			

			// Unhook propertyChanged eventhandlers for TaskCode
			if (TaskCode != null) TaskCode.PropertyChanged -= TaskCode_PropertyChanged;
			TaskCode = _originalTaskCode == null ? null : GenericCopier<ObservablePickListItem>.DeepCopy(_originalTaskCode);
			// Hookup propertyChanged eventhandlers for TaskCode
			if (TaskCode != null) TaskCode.PropertyChanged += TaskCode_PropertyChanged;
			

			// Unhook propertyChanged eventhandlers for TimeEntries
			if (TimeEntries != null) ListUtility.AttachPropertyChangedEventHandlers(TimeEntries,TimeEntries_Item_PropertyChanged, attach:false);
			TimeEntries = _originalTimeEntries == null ? null : GenericCopier<ObservableCollection<ObservableTimeEntry>>.DeepCopy(_originalTimeEntries);
			// Hookup propertyChanged eventhandlers for TimeEntries
			if (TimeEntries != null) ListUtility.AttachPropertyChangedEventHandlers(TimeEntries,TimeEntries_Item_PropertyChanged, attach:true);
			

		}
		

		private void ProjectCode_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_changeTracker["ProjectCode"] != ProjectCode.IsChanged)
			{
				_changeTracker["ProjectCode"] = ProjectCode.IsChanged;
				OnPropertyChanged("IsChanged");
			}
		}
		private void TaskCode_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_changeTracker["TaskCode"] != TaskCode.IsChanged)
			{
				_changeTracker["TaskCode"] = TaskCode.IsChanged;
				OnPropertyChanged("IsChanged");
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
			_projectCode.AcceptChanges();
			_originalProjectCode = GenericCopier<ObservablePickListItem>.DeepCopy(_projectCode);
			

			_taskCode.AcceptChanges();
			_originalTaskCode = GenericCopier<ObservablePickListItem>.DeepCopy(_taskCode);
			

			foreach(var item in _timeEntries)
			{
				item.AcceptChanges();
			}
			_originalTimeEntries = GenericCopier<ObservableCollection<ObservableTimeEntry>>.DeepCopy(_timeEntries);
			

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
