using Model;
using Shared;
using System;
using System.Collections.Generic;


namespace Model
{
	public class TrackableTimeEntry
	{
		private System.TimeSpan? _loggedTime;
		public System.TimeSpan? LoggedTime
		{
			get
			{
				return _loggedTime;
			}
			set
			{
				_loggedTime = value;
			}
		}
		

		private System.TimeSpan? _extraTime;
		public System.TimeSpan? ExtraTime
		{
			get
			{
				return _extraTime;
			}
			set
			{
				_extraTime = value;
			}
		}
		

		private string _notes;
		public string Notes
		{
			get
			{
				return _notes;
			}
			set
			{
				_notes = value;
			}
		}
		

		private System.Int32? _workDetailId;
		public System.Int32? WorkDetailId
		{
			get
			{
				return _workDetailId;
			}
			set
			{
				_workDetailId = value;
			}
		}
		

	}
}
