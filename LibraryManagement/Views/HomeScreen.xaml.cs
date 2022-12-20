using LibraryManagement.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        private double newBooksSectionWidth;
        private double newBooksSectionHeight;

        public HomeScreen()
        {
            InitializeComponent();
            newBooksSectionWidth = newBooksSection.ActualWidth;
            newBooksSectionHeight = newBooksSection.ActualHeight;
        }
        private void ButtonBorrowBook_Click(object sender, RoutedEventArgs e)
        {
            Window window = new BorrowBook();
            window.ShowDialog();
        }
        private void ButtonReturnBook_Click(object sender, RoutedEventArgs e)
        {
            Window window = new ReturnBook();
            window.ShowDialog();
        }
        private void ButtonCollectFine_Click(object sender, RoutedEventArgs e)
        {
            Window window = new CollectFine();
            window.ShowDialog();
        }

        private void ButtonAuthor_Click(object sender, RoutedEventArgs e)
        {
            Window window = new AuthorScreen();
            window.ShowDialog();
        }

        private void ButtonCategory_Click(object sender, RoutedEventArgs e)
        {
            Window window = new CategoryScreen();
            window.ShowDialog();
        }
    }
}
