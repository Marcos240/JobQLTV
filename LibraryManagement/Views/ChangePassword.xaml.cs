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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void currPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtCurr.Text = currPassword.Password;
        }

        private void newPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtNew.Text = newPassword.Password;
        }

        private void confirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtConfirm.Text = confirmPassword.Password;
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
