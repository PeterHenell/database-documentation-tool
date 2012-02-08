using System;
using System.Windows;
using System.ComponentModel;

namespace ConnectionStringCreatorGUI
{
    
    internal partial class SqlConnectionStringBuilder : INotifyPropertyChanged
    {
        private static readonly SqlConnectionString DefaultValue = new SqlConnectionString { IntegratedSecurity = true, Pooling = false };

        public static readonly DependencyProperty ConnectionStringProperty =
            DependencyProperty.Register("ConnectionString", typeof(SqlConnectionString),
                                        typeof(SqlConnectionStringBuilder),
                                        new FrameworkPropertyMetadata(
                                            DefaultValue,
                                            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                            ConnectionStringChanged));

        public SqlConnectionString ConnectionString
        {
            get { return (SqlConnectionString)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        private static void ConnectionStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var builder = (SqlConnectionStringBuilder)d;
            if (e.NewValue == null)
                builder.Dispatcher.BeginInvoke((Action)(() => d.SetValue(ConnectionStringProperty, DefaultValue)));
            else
                builder.RegisterNewConnectionString((SqlConnectionString)e.NewValue);
        }
        private void RegisterNewConnectionString(SqlConnectionString newValue)
        {
            if (newValue != null)
                newValue.PropertyChanged += ConnectionStringPropertyChanged;
        }
        void ConnectionStringPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GetBindingExpression(ConnectionStringProperty).UpdateSource();
        }

        
        public SqlConnectionStringBuilder()
        {
            InitializeComponent();
            
        }
     
        public event PropertyChangedEventHandler PropertyChanged;
     
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
