using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Communication;
using Model;
using Model.Annotations;

namespace PME
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            var communicationModule = App.AutoFacContainer.Resolve<CommunicationModule>();
        }

        private ObservableTimesheet _timesheet;
        public ObservableTimesheet Timesheet
        {
            get { return _timesheet; }
            set
            { 
                _timesheet = value;
                OnPropertyChanged("Timesheet");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
