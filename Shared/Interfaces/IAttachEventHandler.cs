using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    // All generated Observable classes implement this interface so that when we create cloned copies
    // we can attache event handlers for the OnProperty changed events and maintain notifications throughout 
    // the object graph
    public interface IAttachEventHandler
    {
        void AttachEventHandlers();
    }
}
