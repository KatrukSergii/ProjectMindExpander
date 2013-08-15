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
	public partial class ObservableDummyTestObject : INotifyPropertyChanged, IChangeTracking, ICloneable, IAttachEventHandler
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservableDummyTestObject()
		{
			InitializeChangeTracker();
			_isTrackingEnabled = false;
			

			Title = default(string);
			TimesheetId = default(string);
			ProjectTimeItems = null;
			NonProjectActivityItems = null;
			RequiredHours = null;
			TotalRequiredHours = new TimeSpan();
			DummyTimeEntry = new ObservableTimeEntry();
			DummyValueTypeCollection = null;
			_isTrackingEnabled = true;
		}
		

		public ObservableDummyTestObject(DummyTestObject dummyTestObject) : this()
		{
			_isTrackingEnabled = false;
			

			OriginalTitle = dummyTestObject.Title;
			OriginalTimesheetId = dummyTestObject.TimesheetId;
			OriginalProjectTimeItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(dummyTestObject.ProjectTimeItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			OriginalNonProjectActivityItems = new ObservableCollection<ObservableProjectTaskTimesheetItem>(dummyTestObject.NonProjectActivityItems.Select(x => new ObservableProjectTaskTimesheetItem(x)).ToList());
			OriginalRequiredHours = new ObservableCollection<TimeSpan>(dummyTestObject.RequiredHours);
			OriginalTotalRequiredHours = dummyTestObject.TotalRequiredHours;
			OriginalDummyTimeEntry = new ObservableTimeEntry(dummyTestObject.DummyTimeEntry);
			OriginalDummyValueTypeCollection = new ObservableCollection<int>(dummyTestObject.DummyValueTypeCollection);
			

			// Set the properties to the Original property values
			ResetProperties();
			ProjectTimeItems.CollectionChanged += ProjectTimeItems_CollectionChanged;
			NonProjectActivityItems.CollectionChanged += NonProjectActivityItems_CollectionChanged;
			RequiredHours.CollectionChanged += RequiredHours_CollectionChanged;
			DummyValueTypeCollection.CollectionChanged += DummyValueTypeCollection_CollectionChanged;
			DummyTimeEntry.PropertyChanged += DummyTimeEntry_PropertyChanged;
			ResetChangeTracking();
			_isTrackingEnabled = true;
			Initialize();
		}
		

		partial void Initialize();
		

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
							_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(OriginalProjectTimeItems,ProjectTimeItems);
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
					_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(OriginalProjectTimeItems,ProjectTimeItems);
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
				_changeTracker["ProjectTimeItems"] = !ListUtility.EqualTo(OriginalProjectTimeItems,ProjectTimeItems);
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
							_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(OriginalNonProjectActivityItems,NonProjectActivityItems);
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
					_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(OriginalNonProjectActivityItems,NonProjectActivityItems);
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
				_changeTracker["NonProjectActivityItems"] = !ListUtility.EqualTo(OriginalNonProjectActivityItems,NonProjectActivityItems);
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
							_changeTracker["RequiredHours"] = !ListUtility.EqualTo(OriginalRequiredHours,RequiredHours);
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
					_changeTracker["RequiredHours"] = !ListUtility.EqualTo(OriginalRequiredHours,RequiredHours);
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
				_changeTracker["RequiredHours"] = !ListUtility.EqualTo(OriginalRequiredHours,RequiredHours);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private void DummyValueTypeCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
							propertyChangedItem.PropertyChanged -= DummyValueTypeCollection_Item_PropertyChanged;
							// always treat a remove or replace on a reference type as a change
							_changeTracker["ProjectTimeItems"] = true;
						}
						else
						{
							_changeTracker["DummyValueTypeCollection"] = !ListUtility.EqualTo(OriginalDummyValueTypeCollection,DummyValueTypeCollection);
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
							propertyChangedItem.PropertyChanged +=DummyValueTypeCollection_Item_PropertyChanged;
						}
					}
					_changeTracker["DummyValueTypeCollection"] = !ListUtility.EqualTo(OriginalDummyValueTypeCollection,DummyValueTypeCollection);
					OnPropertyChanged("IsChanged");
					break;
			}
		}
		

		private void DummyValueTypeCollection_Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var trackingItem = sender as IChangeTracking;
			if (trackingItem != null)
			{
				_changeTracker["DummyValueTypeCollection"] = trackingItem.IsChanged;
				OnPropertyChanged("IsChanged");
			}
			else
			{
				_changeTracker["DummyValueTypeCollection"] = !ListUtility.EqualTo(OriginalDummyValueTypeCollection,DummyValueTypeCollection);
				OnPropertyChanged("IsChanged");
			}
		}
		

		private string _title;
		public string OriginalTitle { get; private set; }
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
					if ((OriginalTitle == null && value != null) || (OriginalTitle != null && !OriginalTitle.Equals(_title)))
					{
						_changeTracker["Title"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Title"] = false;
					}
					OnPropertyChanged("Title");
				}
			}
		}
		

		private string _timesheetId;
		public string OriginalTimesheetId { get; private set; }
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
					if ((OriginalTimesheetId == null && value != null) || (OriginalTimesheetId != null && !OriginalTimesheetId.Equals(_timesheetId)))
					{
						_changeTracker["TimesheetId"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TimesheetId"] = false;
					}
					OnPropertyChanged("TimesheetId");
				}
			}
		}
		

		private ObservableCollection<ObservableProjectTaskTimesheetItem> _projectTimeItems;
		public ObservableCollection<ObservableProjectTaskTimesheetItem> OriginalProjectTimeItems { get; private set; }
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
					if ((OriginalProjectTimeItems == null && value != null) || (OriginalProjectTimeItems != null && !OriginalProjectTimeItems.Equals(_projectTimeItems)))
					{
						_changeTracker["ProjectTimeItems"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ProjectTimeItems"] = false;
					}
					OnPropertyChanged("ProjectTimeItems");
				}
			}
		}
		

		private ObservableCollection<ObservableProjectTaskTimesheetItem> _nonProjectActivityItems;
		public ObservableCollection<ObservableProjectTaskTimesheetItem> OriginalNonProjectActivityItems { get; private set; }
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
					if ((OriginalNonProjectActivityItems == null && value != null) || (OriginalNonProjectActivityItems != null && !OriginalNonProjectActivityItems.Equals(_nonProjectActivityItems)))
					{
						_changeTracker["NonProjectActivityItems"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["NonProjectActivityItems"] = false;
					}
					OnPropertyChanged("NonProjectActivityItems");
				}
			}
		}
		

		private ObservableCollection<TimeSpan> _requiredHours;
		public ObservableCollection<TimeSpan> OriginalRequiredHours { get; private set; }
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
					if ((OriginalRequiredHours == null && value != null) || (OriginalRequiredHours != null && !OriginalRequiredHours.Equals(_requiredHours)))
					{
						_changeTracker["RequiredHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["RequiredHours"] = false;
					}
					OnPropertyChanged("RequiredHours");
				}
			}
		}
		

		private TimeSpan _totalRequiredHours;
		public TimeSpan OriginalTotalRequiredHours { get; private set; }
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
					if ((OriginalTotalRequiredHours == null && value != null) || (OriginalTotalRequiredHours != null && !OriginalTotalRequiredHours.Equals(_totalRequiredHours)))
					{
						_changeTracker["TotalRequiredHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["TotalRequiredHours"] = false;
					}
					OnPropertyChanged("TotalRequiredHours");
				}
			}
		}
		

		private ObservableTimeEntry _dummyTimeEntry;
		public ObservableTimeEntry OriginalDummyTimeEntry { get; private set; }
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
					if ((OriginalDummyTimeEntry == null && value != null) || (OriginalDummyTimeEntry != null && !OriginalDummyTimeEntry.Equals(_dummyTimeEntry)))
					{
						_changeTracker["DummyTimeEntry"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["DummyTimeEntry"] = false;
					}
					OnPropertyChanged("DummyTimeEntry");
				}
			}
		}
		

		private ObservableCollection<int> _dummyValueTypeCollection;
		public ObservableCollection<int> OriginalDummyValueTypeCollection { get; private set; }
		public ObservableCollection<int> DummyValueTypeCollection
		{
			get
			{
				return _dummyValueTypeCollection;
			}
			set
			{
				if (_dummyValueTypeCollection != value)
				{
					_dummyValueTypeCollection = value;
					if ((OriginalDummyValueTypeCollection == null && value != null) || (OriginalDummyValueTypeCollection != null && !OriginalDummyValueTypeCollection.Equals(_dummyValueTypeCollection)))
					{
						_changeTracker["DummyValueTypeCollection"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["DummyValueTypeCollection"] = false;
					}
					OnPropertyChanged("DummyValueTypeCollection");
				}
			}
		}
		

		private void ResetProperties()
		{
			Title = OriginalTitle;
			

			TimesheetId = OriginalTimesheetId;
			

			// Unhook propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null) ListUtility.AttachPropertyChangedEventHandlers(ProjectTimeItems,ProjectTimeItems_Item_PropertyChanged, attach:false);
			ProjectTimeItems = OriginalProjectTimeItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(OriginalProjectTimeItems);
			// Hookup propertyChanged eventhandlers for ProjectTimeItems
			if (ProjectTimeItems != null) ListUtility.AttachPropertyChangedEventHandlers(ProjectTimeItems,ProjectTimeItems_Item_PropertyChanged, attach:true);
			

			// Unhook propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null) ListUtility.AttachPropertyChangedEventHandlers(NonProjectActivityItems,NonProjectActivityItems_Item_PropertyChanged, attach:false);
			NonProjectActivityItems = OriginalNonProjectActivityItems == null ? null : GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(OriginalNonProjectActivityItems);
			// Hookup propertyChanged eventhandlers for NonProjectActivityItems
			if (NonProjectActivityItems != null) ListUtility.AttachPropertyChangedEventHandlers(NonProjectActivityItems,NonProjectActivityItems_Item_PropertyChanged, attach:true);
			

			RequiredHours = OriginalRequiredHours == null ? null : GenericCopier<ObservableCollection<TimeSpan>>.DeepCopy(OriginalRequiredHours);
			

			TotalRequiredHours = OriginalTotalRequiredHours;
			

			// Unhook propertyChanged eventhandlers for DummyTimeEntry
			if (DummyTimeEntry != null) DummyTimeEntry.PropertyChanged -= DummyTimeEntry_PropertyChanged;
			DummyTimeEntry = OriginalDummyTimeEntry == null ? null : GenericCopier<ObservableTimeEntry>.DeepCopy(OriginalDummyTimeEntry);
			// Hookup propertyChanged eventhandlers for DummyTimeEntry
			if (DummyTimeEntry != null) DummyTimeEntry.PropertyChanged += DummyTimeEntry_PropertyChanged;
			

			DummyValueTypeCollection = OriginalDummyValueTypeCollection == null ? null : GenericCopier<ObservableCollection<int>>.DeepCopy(OriginalDummyValueTypeCollection);
			

		}
		

		private void DummyTimeEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_changeTracker["DummyTimeEntry"] != DummyTimeEntry.IsChanged)
			{
				_changeTracker["DummyTimeEntry"] = DummyTimeEntry.IsChanged;
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
			OriginalTitle = _title;
			

			OriginalTimesheetId = _timesheetId;
			

			foreach(var item in _projectTimeItems)
			{
				item.AcceptChanges();
			}
			OriginalProjectTimeItems = GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_projectTimeItems);
			

			foreach(var item in _nonProjectActivityItems)
			{
				item.AcceptChanges();
			}
			OriginalNonProjectActivityItems = GenericCopier<ObservableCollection<ObservableProjectTaskTimesheetItem>>.DeepCopy(_nonProjectActivityItems);
			

			OriginalRequiredHours = GenericCopier<ObservableCollection<TimeSpan>>.DeepCopy(_requiredHours);
			

			OriginalTotalRequiredHours = _totalRequiredHours;
			

			_dummyTimeEntry.AcceptChanges();
			OriginalDummyTimeEntry = GenericCopier<ObservableTimeEntry>.DeepCopy(_dummyTimeEntry);
			

			OriginalDummyValueTypeCollection = GenericCopier<ObservableCollection<int>>.DeepCopy(_dummyValueTypeCollection);
			

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
			_changeTracker["DummyValueTypeCollection"] = false;
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
			var clone = new ObservableDummyTestObject();
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
			

			clone.DummyTimeEntry = (ObservableTimeEntry)DummyTimeEntry.Clone();
			

			clone.DummyValueTypeCollection = new ObservableCollection<int>();
			foreach(var item in DummyValueTypeCollection)
			{
				clone.DummyValueTypeCollection.Add(item);
			}
			

			clone.AttachEventHandlers();
			clone.AcceptChanges();
			return clone;
		}
		

		// This is only called after Clone() (so no need to unhook handlers). Need to refactor so that ResetProperties calls this
		public void AttachEventHandlers()
		{
			foreach(var item in ProjectTimeItems)
			{
				item.PropertyChanged += ProjectTimeItems_Item_PropertyChanged;
				item.AttachEventHandlers();
			}
			foreach(var item in NonProjectActivityItems)
			{
				item.PropertyChanged += NonProjectActivityItems_Item_PropertyChanged;
				item.AttachEventHandlers();
			}
			RequiredHours.CollectionChanged += RequiredHours_CollectionChanged;
			DummyTimeEntry.PropertyChanged += DummyTimeEntry_PropertyChanged;
			DummyTimeEntry.AttachEventHandlers();
			DummyValueTypeCollection.CollectionChanged += DummyValueTypeCollection_CollectionChanged;
		}
	}
}
