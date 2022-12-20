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

namespace LibraryManagement
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
            tenTaiKhoan.TabIndex = 0;
            matKhau.TabIndex = 1;
            btnLogin.TabIndex = 2;
        }

        private void matKhau_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passWord.Text = matKhau.Password;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnLogin.IsEnabled)
            {
                if (e.Key == Key.Enter)
                {
                    btnLogin.Command.Execute(null);
                }    
            }    
        }
    }
}
