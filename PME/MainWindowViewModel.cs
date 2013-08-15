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
            Timesheet.CalculateTotals();
            Timesheet.PropertyChanged += Timesheet_PropertyChanged;
        }

        private void Timesheet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                Timesheet.CalculateTotals();
            }
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
