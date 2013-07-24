using Model;
using Shared;
using System;
using System.Collections.Generic;
namespace Model
{
	public class TrackableTimesheet
	{
		private string _title;
		private string _timesheetId;
		private List<ProjectTaskTimesheetItem> _projectTimeItems;
		private List<ProjectTaskTimesheetItem> _nonProjectActivityItems;
		private List<TimeSpan> _requiredHours;
		private TimeSpan _totalRequiredHours;
		private bool _isChanged;
	}
}
