using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringCreatorGUI
{
    internal class ConnectionStringBuilderWindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public DelegateCommand OkClickCommand { get; private set; }

        // Action to perform when the window closes
        private Action<SqlConnectionString> _exitReturnAction;

        private SqlConnectionString _connString;
        public SqlConnectionString ConnectionString { 
            get 
            {
                return _connString;
            }
            set {
                
                if (_connString == value)
                    return;
                
                _connString = value;
                OnPropertyChanged("ConnectionString");
            }
        }

        bool _isOpen = true;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            private set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }


        public ConnectionStringBuilderWindowViewModel(ConnectionStringCreatorGUI.SqlConnectionString connStr, Action<SqlConnectionString> action)
        {
            ConnectionString = connStr ?? new SqlConnectionString();
            _exitReturnAction = action;

            OkClickCommand = new DelegateCommand(() =>
                {
                    string connectionString = ConnectionString.ToString();
                    IsOpen = false;
                    _exitReturnAction(ConnectionString);
                });
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

    }
}
