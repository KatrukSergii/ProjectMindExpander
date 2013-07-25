using Model;
using Shared;
using System;
using System.Collections.Generic;


namespace Model
{
	public class TrackableProjectTaskTimesheetItem
	{
		private PickListItem _projectCode;
		public PickListItem ProjectCode
		{
			get
			{
				return _projectCode;
			}
			set
			{
				_projectCode = value;
			}
		}
		

		private PickListItem _taskCode;
		public PickListItem TaskCode
		{
			get
			{
				return _taskCode;
			}
			set
			{
				_taskCode = value;
			}
		}
		

		private List<TimeEntry> _timeEntries;
		public List<TimeEntry> TimeEntries
		{
			get
			{
				return _timeEntries;
			}
			set
			{
				_timeEntries = value;
			}
		}
		

	}
}
