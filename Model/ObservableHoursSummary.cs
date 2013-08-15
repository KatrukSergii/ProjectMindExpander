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
	public partial class ObservableHoursSummary : INotifyPropertyChanged, IChangeTracking, ICloneable, IAttachEventHandler
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservableHoursSummary()
		{
			InitializeChangeTracker();
			_isTrackingEnabled = false;
			

			CoreHours = new TimeSpan();
			ExtraHours = new TimeSpan();
			LoggedHours = new TimeSpan();
			RemainingCoreHours = new TimeSpan();
			_isTrackingEnabled = true;
		}
		

		public ObservableHoursSummary(HoursSummary hoursSummary) : this()
		{
			_isTrackingEnabled = false;
			

			OriginalCoreHours = hoursSummary.CoreHours;
			OriginalExtraHours = hoursSummary.ExtraHours;
			OriginalLoggedHours = hoursSummary.LoggedHours;
			OriginalRemainingCoreHours = hoursSummary.RemainingCoreHours;
			

			// Set the properties to the Original property values
			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
			Initialize();
		}
		

		partial void Initialize();
		

		private TimeSpan _coreHours;
		public TimeSpan OriginalCoreHours { get; private set; }
		public TimeSpan CoreHours
		{
			get
			{
				return _coreHours;
			}
			set
			{
				if (_coreHours != value)
				{
					_coreHours = value;
					if ((OriginalCoreHours == null && value != null) || (OriginalCoreHours != null && !OriginalCoreHours.Equals(_coreHours)))
					{
						_changeTracker["CoreHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["CoreHours"] = false;
					}
					OnPropertyChanged("CoreHours");
				}
			}
		}
		

		private TimeSpan _extraHours;
		public TimeSpan OriginalExtraHours { get; private set; }
		public TimeSpan ExtraHours
		{
			get
			{
				return _extraHours;
			}
			set
			{
				if (_extraHours != value)
				{
					_extraHours = value;
					if ((OriginalExtraHours == null && value != null) || (OriginalExtraHours != null && !OriginalExtraHours.Equals(_extraHours)))
					{
						_changeTracker["ExtraHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ExtraHours"] = false;
					}
					OnPropertyChanged("ExtraHours");
				}
			}
		}
		

		private TimeSpan _loggedHours;
		public TimeSpan OriginalLoggedHours { get; private set; }
		public TimeSpan LoggedHours
		{
			get
			{
				return _loggedHours;
			}
			set
			{
				if (_loggedHours != value)
				{
					_loggedHours = value;
					if ((OriginalLoggedHours == null && value != null) || (OriginalLoggedHours != null && !OriginalLoggedHours.Equals(_loggedHours)))
					{
						_changeTracker["LoggedHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["LoggedHours"] = false;
					}
					OnPropertyChanged("LoggedHours");
				}
			}
		}
		

		private TimeSpan _remainingCoreHours;
		public TimeSpan OriginalRemainingCoreHours { get; private set; }
		public TimeSpan RemainingCoreHours
		{
			get
			{
				return _remainingCoreHours;
			}
			set
			{
				if (_remainingCoreHours != value)
				{
					_remainingCoreHours = value;
					if ((OriginalRemainingCoreHours == null && value != null) || (OriginalRemainingCoreHours != null && !OriginalRemainingCoreHours.Equals(_remainingCoreHours)))
					{
						_changeTracker["RemainingCoreHours"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["RemainingCoreHours"] = false;
					}
					OnPropertyChanged("RemainingCoreHours");
				}
			}
		}
		

		private void ResetProperties()
		{
			CoreHours = OriginalCoreHours;
			

			ExtraHours = OriginalExtraHours;
			

			LoggedHours = OriginalLoggedHours;
			

			RemainingCoreHours = OriginalRemainingCoreHours;
			

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
			OriginalCoreHours = _coreHours;
			

			OriginalExtraHours = _extraHours;
			

			OriginalLoggedHours = _loggedHours;
			

			OriginalRemainingCoreHours = _remainingCoreHours;
			

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
			_changeTracker["CoreHours"] = false;
			_changeTracker["ExtraHours"] = false;
			_changeTracker["LoggedHours"] = false;
			_changeTracker["RemainingCoreHours"] = false;
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
			var clone = new ObservableHoursSummary();
			clone.CoreHours = CoreHours;
			

			clone.ExtraHours = ExtraHours;
			

			clone.LoggedHours = LoggedHours;
			

			clone.RemainingCoreHours = RemainingCoreHours;
			

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
