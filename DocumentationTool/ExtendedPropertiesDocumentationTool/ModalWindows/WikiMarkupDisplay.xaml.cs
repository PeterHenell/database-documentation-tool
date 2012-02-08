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
    /// Interaction logic for WikiMarkupDisplay.xaml
    /// </summary>
    public partial class WikiMarkupDisplay : Window
    {
        public WikiMarkupDisplay()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(WikiMarkupDisplay_Loaded);
        }

        void WikiMarkupDisplay_Loaded(object sender, RoutedEventArgs e)
        {
         
        }

        public void Show(string text)
        {
            WikiTextbox.Text = text;
            WikiTextbox.Focus();
            WikiTextbox.SelectAll();
            base.Show();
        }
    }
}
