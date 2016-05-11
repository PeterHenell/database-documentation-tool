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
using System.Windows.Shapes;

namespace ExtendedPropertiesDocumentationTool.ModalWindows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        ApplicationSettings ApplicationSettings { get; set; }

        public ConfigurationWindow(ApplicationSettings appSettings)
        {
            InitializeComponent();
            this.ApplicationSettings = appSettings;
            this.Loaded += ConfigurationWindow_Loaded;
        }

        void ConfigurationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.ApplicationSettings;           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ApplicationSettings.Save();
            this.Close();
        }
    }
}
