using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model.Annotations;

namespace PME
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set 
            { 
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
    }
}
