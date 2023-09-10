using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace folder_cacher_net
{
    /// <summary>
    /// Interaction logic for folder_entry.xaml
    /// </summary>
    public partial class folder_entry : UserControl
    {
        public folder_entry()
        {
            InitializeComponent();
        }


        private void WriteStatus(string _text)
        {
            txt_Status.Text = _text;   
        }

        private async void Cache()
        {
            if (!Directory.Exists(txt_Path.Text))
            {
                WriteStatus("Error: Directory path is not exist.");
                return;
            }


        }


        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Resume_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Cache_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
