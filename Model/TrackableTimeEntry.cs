using Model;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Model
{
	public class TrackableTimeEntry : INotifyPropertyChanged, IChangeTracking
	{
		public TrackableTimeEntry()
		{
			LoggedTime = null;
			ExtraTime = null;
			Notes = default(string);
			WorkDetailId = null;
		}
		

		public TrackableTimeEntry(TimeEntry timeEntry)
		{
			_originalLoggedTime = timeEntry.LoggedTime;
			_originalExtraTime = timeEntry.ExtraTime;
			_originalNotes = timeEntry.Notes;
			_originalWorkDetailId = timeEntry.WorkDetailId;
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
		}
		
		
		private bool _isChanged;
		public bool IsChanged
		{
			get 
			{ 
				return _isChanged;
			}
			set
			{
				if (_isChanged != value)
				{
					_isChanged = value;
					OnPropertyChanged("IsChanged");
				}
			}
		}
				
		#endregion
	}
}
