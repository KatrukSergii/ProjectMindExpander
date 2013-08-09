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
			_originalProjectTimeItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(timesheet.ProjectTimeItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalNonProjectActivityItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(timesheet.NonProjectActivityItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalRequiredHours = new ObservableCollection<TimeSpan>(timesheet.RequiredHours);
			_originalTotalRequiredHours = timesheet.TotalRequiredHours;
			_originalDummyTimeEntry = new ObservableTimeEntry(timesheet.DummyTimeEntry);
			

			// Set the properties to the _original property values
			ResetProperties();
			ProjectTimeItems.CollectionChanged += ProjectTimeItems_CollectionChanged;
			NonProjectActivityItems.CollectionChanged += NonProjectActivityItems_CollectionChanged;
			RequiredHours.CollectionChanged += RequiredHours_CollectionChanged;
			//TODO hook up  all of the property changed events for non-collection reference types
			foreach(var item in ProjectTimeItems)
			{
				var propertyChangedItem = item as INotifyPropertyChanged;
				if(propertyChangedItem != null)
				{
					propertyChangedItem.PropertyChanged += ProjectTimeItems_Item_PropertyChanged;
				}
			}
			

			foreach(var item in NonProjectActivityItems)
			{
				var propertyChangedItem = item as INotifyPropertyChanged;
				if(propertyChangedItem != null)
				{
					propertyChangedItem.PropertyChanged += NonProjectActivityItems_Item_PropertyChanged;
				}
			}
			

			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private void ProjectTimeItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_changeTracker["ProjectTimeItems"] = true;
			

			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						var propertyChangedItem = item as INotifyPropertyChanged;
						if (propertyChangedItem != null)
						{
							propertyChangedItem.PropertyChanged -= ProjectTimeItems_Item_PropertyChanged;
						}
						else
						{
							_changeTracker["ProjectTimeItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
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
							propertyChangedItem.PropertyChanged +=ProjectTimeItems_Item_PropertyChanged;
						}
					}
					_changeTracker["ProjectTimeItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
					OnPropertyChanged("IsChanged");
					break;
			}
		}
		

		private void ProjectTimeItems_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var trackingItem = sender as IChangeTracking;
			if (trackingItem != null)
			{
				_changeTracker["ProjectTimeItems"] = trackingItem.IsChanged;
				OnPropertyChanged("IsChanged");
			}
			else
			{
				_changeTracker["ProjectTimeItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private void NonProjectActivityItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_changeTracker["NonProjectActivityItems"] = true;
			

			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						var propertyChangedItem = item as INotifyPropertyChanged;
						if (propertyChangedItem != null)
						{
							propertyChangedItem.PropertyChanged -= NonProjectActivityItems_Item_PropertyChanged;
						}
						else
						{
							_changeTracker["NonProjectActivityItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
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
							propertyChangedItem.PropertyChanged +=NonProjectActivityItems_Item_PropertyChanged;
						}
					}
					_changeTracker["NonProjectActivityItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
					OnPropertyChanged("IsChanged");
					break;
			}
		}
		

		private void NonProjectActivityItems_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var trackingItem = sender as IChangeTracking;
			if (trackingItem != null)
			{
				_changeTracker["NonProjectActivityItems"] = trackingItem.IsChanged;
				OnPropertyChanged("IsChanged");
			}
			else
			{
				_changeTracker["NonProjectActivityItems"] = !ListUtilities<ObservableProjectTaskTimesheetItem>.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private void RequiredHours_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_changeTracker["RequiredHours"] = true;
			

			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						var propertyChangedItem = item as INotifyPropertyChanged;
						if (propertyChangedItem != null)
						{
							propertyChangedItem.PropertyChanged -= RequiredHours_Item_PropertyChanged;
						}
						else
						{
							_changeTracker["RequiredHours"] = !ListUtilities<TimeSpan>.EqualTo(_originalRequiredHours,RequiredHours);
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
							propertyChangedItem.PropertyChanged +=RequiredHours_Item_PropertyChanged;
						}
					}
					_changeTracker["RequiredHours"] = !ListUtilities<TimeSpan>.EqualTo(_originalRequiredHours,RequiredHours);
					OnPropertyChanged("IsChanged");
					break;
			}
		}
		

		private void RequiredHours_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var trackingItem = sender as IChangeTracking;
			if (trackingItem != null)
			{
				_changeTracker["RequiredHours"] = trackingItem.IsChanged;
				OnPropertyChanged("IsChanged");
			}
			else
			{
				_changeTracker["RequiredHours"] = !ListUtilities<TimeSpan>.EqualTo(_originalRequiredHours,RequiredHours);
				OnPropertyChanged("IsChanged");
			}
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
		

		private ObservableCollection<ObservableProjectTaskTimesheetItem> _projectTimeItems;
		private ObservableCollection<ObservableProjectTaskTimesheetItem> _originalProjectTimeItems;
		public ObservableCollection<ObservableProjectTaskTimesheetItem> ProjectTimeItems
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
		

		private ObservableCollection<ObservableProjectTaskTimesheetItem> _nonProjectActivityItems;
		private ObservableCollection<ObservableProjectTaskTimesheetItem> _originalNonProjectActivityItems;
		public ObservableCollection<ObservableProjectTaskTimesheetItem> NonProjectActivityItems
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
		

		private ObservableCollection<TimeSpan> _requiredHours;
		private ObservableCollection<TimeSpan> _originalRequiredHours;
		public ObservableCollection<TimeSpan> RequiredHours
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
			// Unhook propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null)
			{
				foreach(var item in ProjectTimeItems)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged -= ProjectTimeItems_Item_PropertyChanged;
					}
				}
			}
			ProjectTimeItems = _originalProjectTimeItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_originalProjectTimeItems);
			// Hook-up propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null)
			{
				foreach(var item in ProjectTimeItems)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged += ProjectTimeItems_Item_PropertyChanged;
					}
				}
			}
			// Unhook propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null)
			{
				foreach(var item in NonProjectActivityItems)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged -= NonProjectActivityItems_Item_PropertyChanged;
					}
				}
			}
			NonProjectActivityItems = _originalNonProjectActivityItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_originalNonProjectActivityItems);
			// Hook-up propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null)
			{
				foreach(var item in NonProjectActivityItems)
				{
					var propertyChangedItem = item as INotifyPropertyChanged;
					if(propertyChangedItem != null)
					{
						propertyChangedItem.PropertyChanged += NonProjectActivityItems_Item_PropertyChanged;
					}
				}
			}
			RequiredHours = _originalRequiredHours == null ? null : GenericCopier<ObservableCollection<TimeSpan>>.DeepCopy(_originalRequiredHours);
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
