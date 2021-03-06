﻿using Shared.Utility;
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
	public partial class ObservablePickListItem : INotifyPropertyChanged, IChangeTracking, ICloneable, IAttachEventHandler
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservablePickListItem()
		{
			InitializeChangeTracker();
			_isTrackingEnabled = false;
			

			Value = default(int);
			Name = default(string);
			_isTrackingEnabled = true;
		}
		

		public ObservablePickListItem(PickListItem pickListItem) : this()
		{
			_isTrackingEnabled = false;
			

			OriginalValue = pickListItem.Value;
			OriginalName = pickListItem.Name;
			

			// Set the properties to the Original property values
			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
			Initialize();
		}
		

		partial void Initialize();
		

		private int _value;
		public int OriginalValue { get; private set; }
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
					if ((OriginalValue == null && value != null) || (OriginalValue != null && !OriginalValue.Equals(_value)))
					{
						_changeTracker["Value"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Value"] = false;
					}
					OnPropertyChanged("Value");
				}
			}
		}
		

		private string _name;
		public string OriginalName { get; private set; }
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
					if ((OriginalName == null && value != null) || (OriginalName != null && !OriginalName.Equals(_name)))
					{
						_changeTracker["Name"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Name"] = false;
					}
					OnPropertyChanged("Name");
				}
			}
		}
		

		private void ResetProperties()
		{
			Value = OriginalValue;
			

			Name = OriginalName;
			

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
			OriginalValue = _value;
			

			OriginalName = _name;
			

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
			var clone = new ObservablePickListItem();
			clone.Value = Value;
			

			clone.Name = Name;
			

			clone.AttachEventHandlers();
			clone.AcceptChanges();
			return clone;
		}
		

		// This is only called after Clone() (so no need to unhook handlers). Need to refactor so that ResetProperties calls this
		public void AttachEventHandlers()
		{
		}
	}
}
