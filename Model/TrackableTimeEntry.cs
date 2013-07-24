using Model;
using Shared;
using System;
using System.Collections.Generic;
namespace Model
{
	public class TrackableTimeEntry
	{
		private System.TimeSpan? _loggedTime;
		private System.TimeSpan? _extraTime;
		private string _notes;
		private System.Int32? _workDetailId;
	}
}
