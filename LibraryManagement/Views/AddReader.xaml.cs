using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for AddMember.xaml
    /// </summary>
    public partial class AddReader : Window
    {
        public AddReader()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameReader.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Email.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Address.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            DobReader.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
        }
    }
}
