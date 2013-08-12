using Shared.Utility;
using Shared.Interfaces;
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
	public partial class ObservableTimesheet : INotifyPropertyChanged, IChangeTracking, ICloneable, IAttachEventHandler
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
		}
		public ObservableTimesheet(Timesheet timesheet) : this()
		{
			_originalTitle = timesheet.Title;
			_originalTimesheetId = timesheet.TimesheetId;
			_originalProjectTimeItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(timesheet.ProjectTimeItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalNonProjectActivityItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(timesheet.NonProjectActivityItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			_originalRequiredHours = new ObservableCollection<TimeSpan>(timesheet.RequiredHours);
			_originalTotalRequiredHours = timesheet.TotalRequiredHours;
			

			// Set the properties to the _original property values
			ResetProperties();
			ProjectTimeItems.CollectionChanged += ProjectTimeItems_CollectionChanged;
			NonProjectActivityItems.CollectionChanged += NonProjectActivityItems_CollectionChanged;
			RequiredHours.CollectionChanged += RequiredHours_CollectionChanged;
			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private void ProjectTimeItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
							propertyChangedItem.PropertyChanged -= ProjectTimeItems_Item_PropertyChanged;
							// always treat a remove or replace on a reference type as a change
							_changeTracker["ProjectTimeItems"] = true;
						}
						else
						{
							_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
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
							propertyChangedItem.PropertyChanged +=ProjectTimeItems_Item_PropertyChanged;
						}
					}
					_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
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
				_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(_originalProjectTimeItems,ProjectTimeItems);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private void NonProjectActivityItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
							propertyChangedItem.PropertyChanged -= NonProjectActivityItems_Item_PropertyChanged;
							// always treat a remove or replace on a reference type as a change
							_changeTracker["ProjectTimeItems"] = true;
						}
						else
						{
							_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
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
							propertyChangedItem.PropertyChanged +=NonProjectActivityItems_Item_PropertyChanged;
						}
					}
					_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
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
				_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(_originalNonProjectActivityItems,NonProjectActivityItems);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private void RequiredHours_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
							propertyChangedItem.PropertyChanged -= RequiredHours_Item_PropertyChanged;
							// always treat a remove or replace on a reference type as a change
							_changeTracker["ProjectTimeItems"] = true;
						}
						else
						{
							_changeTracker["RequiredHours"] = !ListUtility.EqualTo(_originalRequiredHours,RequiredHours);
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
							propertyChangedItem.PropertyChanged +=RequiredHours_Item_PropertyChanged;
						}
					}
					_changeTracker["RequiredHours"] = !ListUtility.EqualTo(_originalRequiredHours,RequiredHours);
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
				_changeTracker["RequiredHours"] = !ListUtility.EqualTo(_originalRequiredHours,RequiredHours);
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
		

		private void ResetProperties()
		{
			Title = _originalTitle;
			

			TimesheetId = _originalTimesheetId;
			

			// Unhook propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null) ListUtility.AttachPropertyChangedEventHandlers(ProjectTimeItems,ProjectTimeItems_Item_PropertyChanged, attach:false);
			ProjectTimeItems = _originalProjectTimeItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_originalProjectTimeItems);
			// Hookup propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null) ListUtility.AttachPropertyChangedEventHandlers(ProjectTimeItems,ProjectTimeItems_Item_PropertyChanged, attach:true);
			

			// Unhook propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null) ListUtility.AttachPropertyChangedEventHandlers(NonProjectActivityItems,NonProjectActivityItems_Item_PropertyChanged, attach:false);
			NonProjectActivityItems = _originalNonProjectActivityItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_originalNonProjectActivityItems);
			// Hookup propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null) ListUtility.AttachPropertyChangedEventHandlers(NonProjectActivityItems,NonProjectActivityItems_Item_PropertyChanged, attach:true);
			

			RequiredHours = _originalRequiredHours == null ? null : GenericCopier<ObservableCollection<TimeSpan>>.DeepCopy(_originalRequiredHours);
			

			TotalRequiredHours = _originalTotalRequiredHours;
			

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
			

			foreach(var item in _projectTimeItems)
			{
				item.AcceptChanges();
			}
			_originalProjectTimeItems = GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_projectTimeItems);
			

			foreach(var item in _nonProjectActivityItems)
			{
				item.AcceptChanges();
			}
			_originalNonProjectActivityItems = GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_nonProjectActivityItems);
			

			_originalRequiredHours = GenericCopier<ObservableCollection<TimeSpan>>.DeepCopy(_requiredHours);
			

			_originalTotalRequiredHours = _totalRequiredHours;
			

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
		

		public object Clone()
		{
			var clone = new ObservableTimesheet();
			clone.Title = Title;
			

			clone.TimesheetId = TimesheetId;
			

			clone.ProjectTimeItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>();
			foreach(var item in ProjectTimeItems)
			{
				clone.ProjectTimeItems.Add(item);
			}
			

			clone.NonProjectActivityItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>();
			foreach(var item in NonProjectActivityItems)
			{
				clone.NonProjectActivityItems.Add(item);
			}
			

			clone.RequiredHours = new ObservableCollection<TimeSpan>();
			foreach(var item in RequiredHours)
			{
				clone.RequiredHours.Add(item);
			}
			

			clone.TotalRequiredHours = TotalRequiredHours;
			

			clone.AttachEventHandlers();
			return clone;
		}
		

		public void AttachEventHandlers()
		{
		}
	}
}
