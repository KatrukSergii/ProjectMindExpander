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
	public class ObservableTimeEntry : INotifyPropertyChanged, IChangeTracking
	{
		private Dictionary<string,bool> _changeTracker;
		

		public ObservableTimeEntry()
		{
			InitializeChangeTracker();
			LoggedTime = null;
			ExtraTime = null;
			Notes = default(string);
			WorkDetailId = null;
		}
		

		public ObservableTimeEntry(TimeEntry timeEntry) : this()
		{
			_originalLoggedTime = timeEntry.LoggedTime;
			_originalExtraTime = timeEntry.ExtraTime;
			_originalNotes = timeEntry.Notes;
			_originalWorkDetailId = timeEntry.WorkDetailId;
			

			LoggedTime = GenericCopier<TimeSpan?>.DeepCopy(timeEntry.LoggedTime);
			ExtraTime = GenericCopier<TimeSpan?>.DeepCopy(timeEntry.ExtraTime);
			Notes = timeEntry.Notes;
			WorkDetailId = GenericCopier<int?>.DeepCopy(timeEntry.WorkDetailId);
			

			ResetChangeTracking();
		}
		

		private TimeSpan? _loggedTime;
		private TimeSpan? _originalLoggedTime;
		public TimeSpan? LoggedTime
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
					OnPropertyChanged("LoggedTime");
					if (_originalLoggedTime != null && !_originalLoggedTime.Equals(_loggedTime))
					{
						_changeTracker["LoggedTime"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["LoggedTime"] = false;
					}
				}
			}
		}
		

		private TimeSpan? _extraTime;
		private TimeSpan? _originalExtraTime;
		public TimeSpan? ExtraTime
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
					OnPropertyChanged("ExtraTime");
					if (_originalExtraTime != null && !_originalExtraTime.Equals(_extraTime))
					{
						_changeTracker["ExtraTime"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["ExtraTime"] = false;
					}
				}
			}
		}
		

		private string _notes;
		private string _originalNotes;
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
					OnPropertyChanged("Notes");
					if (_originalNotes != null && !_originalNotes.Equals(_notes))
					{
						_changeTracker["Notes"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["Notes"] = false;
					}
				}
			}
		}
		

		private int? _workDetailId;
		private int? _originalWorkDetailId;
		public int? WorkDetailId
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
					OnPropertyChanged("WorkDetailId");
					if (_originalWorkDetailId != null && !_originalWorkDetailId.Equals(_workDetailId))
					{
						_changeTracker["WorkDetailId"] = true;
						OnPropertyChanged("IsChanged");
					}
					else
					{
						_changeTracker["WorkDetailId"] = false;
					}
				}
			}
		}
		

		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		
		#endregion

		#region IChangeTracking

		public void AcceptChanges()
		{
			_originalLoggedTime = _loggedTime;
			_originalExtraTime = _extraTime;
			_originalNotes = _notes;
			_originalWorkDetailId = _workDetailId;
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
