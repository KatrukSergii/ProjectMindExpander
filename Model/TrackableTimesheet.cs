using Model;
using Shared;
using System;
using System.Collections.Generic;


namespace Model
{
	public class TrackableTimesheet
	{
		private string _title;
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}
		

		private string _timesheetId;
		public string TimesheetId
		{
			get
			{
				return _timesheetId;
			}
			set
			{
				_timesheetId = value;
			}
		}
		

		private List<ProjectTaskTimesheetItem> _projectTimeItems;
		public List<ProjectTaskTimesheetItem> ProjectTimeItems
		{
			get
			{
				return _projectTimeItems;
			}
			set
			{
				_projectTimeItems = value;
			}
		}
		

		private List<ProjectTaskTimesheetItem> _nonProjectActivityItems;
		public List<ProjectTaskTimesheetItem> NonProjectActivityItems
		{
			get
			{
				return _nonProjectActivityItems;
			}
			set
			{
				_nonProjectActivityItems = value;
			}
		}
		

		private List<TimeSpan> _requiredHours;
		public List<TimeSpan> RequiredHours
		{
			get
			{
				return _requiredHours;
			}
			set
			{
				_requiredHours = value;
			}
		}
		

		private TimeSpan _totalRequiredHours;
		public TimeSpan TotalRequiredHours
		{
			get
			{
				return _totalRequiredHours;
			}
			set
			{
				_totalRequiredHours = value;
			}
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
				_isChanged = value;
			}
		}
		

	}
}
