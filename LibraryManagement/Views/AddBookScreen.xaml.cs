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

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for AddBookScreen.xaml
    /// </summary>
    public partial class AddBookScreen : Window
    {
        public AddBookScreen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dateManufacture.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
            NameBook.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            NameBook.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbPrice.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Category.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            cbAuthor.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
        }
    }
}
