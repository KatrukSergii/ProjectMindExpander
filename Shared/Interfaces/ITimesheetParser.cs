using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface ITimesheetParser
    {
        Timesheet Parse(StreamReader streamReader);
    }
}
