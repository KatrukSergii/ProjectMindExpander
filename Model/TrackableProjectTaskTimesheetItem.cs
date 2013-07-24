using Model;
using Shared;
using System;
using System.Collections.Generic;
namespace Model
{
	public class TrackableProjectTaskTimesheetItem
	{
		private PickListItem _projectCode;
		private PickListItem _taskCode;
		private List<TimeEntry> _timeEntries;
	}
}
