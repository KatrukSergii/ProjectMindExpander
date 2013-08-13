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
	public partial class ObservableTimeEntry : INotifyPropertyChanged, IChangeTracking, ICloneable, IAttachEventHandler
	{
		private Dictionary<string,bool> _changeTracker;
		private bool _isTrackingEnabled;
		

		public ObservableTimeEntry()
		{
			InitializeChangeTracker();
			_isTrackingEnabled = false;
			

			LoggedTime = new TimeSpan();
			ExtraTime = new TimeSpan();
			Notes = default(string);
			WorkDetailId = default(int);
			_isTrackingEnabled = true;
		}
		

		public ObservableTimeEntry(TimeEntry timeEntry) : this()
		{
			_isTrackingEnabled = false;
			

			OriginalLoggedTime = timeEntry.LoggedTime;
			OriginalExtraTime = timeEntry.ExtraTime;
			OriginalNotes = timeEntry.Notes;
			OriginalWorkDetailId = timeEntry.WorkDetailId;
			

			// Set the properties to the Original property values
			ResetProperties();
			ResetChangeTracking();
			_isTrackingEnabled = true;
		}
		

		private TimeSpan _loggedTime;
		public TimeSpan OriginalLoggedTime { get; private set; }
		public TimeSpan LoggedTime
		{
			get
			{
				return _loggedTime;
			}
			set
			{
				if (_loggedTime != value)
				{
					_loggedTime = value;
					if ((OriginalLoggedTime == null && value != null) || (OriginalLoggedTime != null && !OriginalLoggedTime.Equals(_loggedTime)))
					{
						_changeTracker["LoggedTime"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["LoggedTime"] = false;
					}
					OnPropertyChanged("LoggedTime");
				}
			}
		}
		

		private TimeSpan _extraTime;
		public TimeSpan OriginalExtraTime { get; private set; }
		public TimeSpan ExtraTime
		{
			get
			{
				return _extraTime;
			}
			set
			{
				if (_extraTime != value)
				{
					_extraTime = value;
					if ((OriginalExtraTime == null && value != null) || (OriginalExtraTime != null && !OriginalExtraTime.Equals(_extraTime)))
					{
						_changeTracker["ExtraTime"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ExtraTime"] = false;
					}
					OnPropertyChanged("ExtraTime");
				}
			}
		}
		

		private string _notes;
		public string OriginalNotes { get; private set; }
		public string Notes
		{
			get
			{
				return _notes;
			}
			set
			{
				if (_notes != value)
				{
					_notes = value;
					if ((OriginalNotes == null && value != null) || (OriginalNotes != null && !OriginalNotes.Equals(_notes)))
					{
						_changeTracker["Notes"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Notes"] = false;
					}
					OnPropertyChanged("Notes");
				}
			}
		}
		

		private int _workDetailId;
		public int OriginalWorkDetailId { get; private set; }
		public int WorkDetailId
		{
			get
			{
				return _workDetailId;
			}
			set
			{
				if (_workDetailId != value)
				{
					_workDetailId = value;
					if ((OriginalWorkDetailId == null && value != null) || (OriginalWorkDetailId != null && !OriginalWorkDetailId.Equals(_workDetailId)))
					{
						_changeTracker["WorkDetailId"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["WorkDetailId"] = false;
					}
					OnPropertyChanged("WorkDetailId");
				}
			}
		}
		

		private void ResetProperties()
		{
			LoggedTime = OriginalLoggedTime;
			

			ExtraTime = OriginalExtraTime;
			

			Notes = OriginalNotes;
			

			WorkDetailId = OriginalWorkDetailId;
			

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
			OriginalLoggedTime = _loggedTime;
			

			OriginalExtraTime = _extraTime;
			

			OriginalNotes = _notes;
			

			OriginalWorkDetailId = _workDetailId;
			

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
			_changeTracker["LoggedTime"] = false;
			_changeTracker["ExtraTime"] = false;
			_changeTracker["Notes"] = false;
			_changeTracker["WorkDetailId"] = false;
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
			var clone = new ObservableTimeEntry();
			clone.LoggedTime = LoggedTime;
			

			clone.ExtraTime = ExtraTime;
			

			clone.Notes = Notes;
			

			clone.WorkDetailId = WorkDetailId;
			

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
