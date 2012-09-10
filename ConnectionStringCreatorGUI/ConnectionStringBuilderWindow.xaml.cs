using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConnectionStringCreatorGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConnectionStringBuilderWindow : Window
    {
        ConnectionStringBuilderWindowViewModel _viewModel;

        public ConnectionStringBuilderWindow(ConnectionStringCreatorGUI.SqlConnectionString connStr, Action<SqlConnectionString> action)
        {
            SqlConnectionString con = new SqlConnectionString();
            InitializeComponent();
            _viewModel = new ConnectionStringBuilderWindowViewModel(connStr, action);

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        public ConnectionStringBuilderWindow(ConnectionStringCreatorGUI.SqlConnectionString connStr
            , Control[] content                                    
            , Action<SqlConnectionString> action) : this(connStr, action)
            
        {
            foreach (var item in content)
            {
                ContentStackPanel.Children.Add(item);
            }
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _viewModel;
            this.Activate(); // To make this window focused
        }
    }
}
