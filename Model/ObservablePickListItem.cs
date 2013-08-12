﻿using Shared.Utility;
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
	public partial class ObservablePickListItem : INotifyPropertyChanged, IChangeTracking, ICloneable
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservablePickListItem()
		{
			InitializeChangeTracker();
			Value = default(int);
			Name = default(string);
		}
		public ObservablePickListItem(PickListItem pickListItem) : this()
		{
			_originalValue = pickListItem.Value;
			_originalName = pickListItem.Name;
			

			// Set the properties to the _original property values
			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private int _value;
		private int _originalValue;
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (_value != value)
				{
					_value = value;
					OnPropertyChanged("Value");
					if (_originalValue == null || !_originalValue.Equals(_value))
					{
						_changeTracker["Value"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Value"] = false;
					}
				}
			}
		}
		

		private string _name;
		private string _originalName;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
					if (_originalName == null || !_originalName.Equals(_name))
					{
						_changeTracker["Name"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Name"] = false;
					}
				}
			}
		}
		

		private void ResetProperties()
		{
			Value = _originalValue;
			

			Name = _originalName;
			

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
			_originalValue = _value;
			

			_originalName = _name;
			

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
			_changeTracker["Value"] = false;
			_changeTracker["Name"] = false;
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
			var clone = default(ObservablePickListItem);
			clone.Value = Value;
			

			clone.Name = Name;
			

			clone.AttachEventHandlers();
			return clone;
		}
		

		public void AttachEventHandlers()
		{
		}
	}
}
//Value Properties:
	//Title -  string
	//TimesheetId -  string
	//TotalRequiredHours -  TimeSpan
	//Value -  int
	//Name -  string
	

//Observable Properties:
	//DummyTimeEntry -  TimeEntry
	//ProjectCode -  PickListItem
	//TaskCode -  PickListItem
	

// Value Type List Properties
	//RequiredHours -  List<TimeSpan>
	//DummyValueTypeCollection -  List<int>
	

// Observable Type List Properties
	//ProjectTimeItems -  List<ProjectTaskTimesheetItem>
	//NonProjectActivityItems -  List<ProjectTaskTimesheetItem>
	//TimeEntries -  List<TimeEntry>
	

