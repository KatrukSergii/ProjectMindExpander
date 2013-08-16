using System;
using System.Windows.Input;
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
            SaveCommand = new DelegateCommand(x => Save(), x => HasChanges());
        }

        public ICommand SaveCommand { get; set; }

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

        public bool HasChanges()
        {
            return (Timesheet != null) && (Timesheet.IsChanged);
        }

        public void Save()
        {
            Timesheet = _webScraper.UpdateTimeSheet(Timesheet);
            Timesheet.AcceptChanges();
        }
    }
}
