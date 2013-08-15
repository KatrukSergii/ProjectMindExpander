using System;
using Communication;
using Model;

namespace PME
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IWebScraper _webScraper;

        public MainWindowViewModel(IWebScraper webScraper)
        {
            _webScraper = webScraper;
            Timesheet = webScraper.LoginAndGetTimesheet();
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

    }
}
