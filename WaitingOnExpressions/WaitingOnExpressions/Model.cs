using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitingOnExpressions
{
    public class Model : NotifyPropertyBase
    {
        public Model()
        {
            Logs = new ObservableCollection<LogEvent>();
        }

        public ObservableCollection<LogEvent> Logs { get; private set; }

        private string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                _keyword = value;
                OnPropertyChanged("Keyword");
            }
        }

        private double _treshold;
        public double Treshold
        {
            get { return _treshold; }
            set
            {
                _treshold = value;
                OnPropertyChanged("Treshold");
            }
        }
    }
    
    public class LogEvent : NotifyPropertyBase
    {
        private DateTime _timestamp;
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                OnPropertyChanged("TimeStamp");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
    }
}
