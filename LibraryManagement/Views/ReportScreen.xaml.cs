using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    /// 

    public partial class ReportScreen : UserControl
    {
        public ReportScreen()
        {
            InitializeComponent();
        }

        private void year_Loaded(object sender, RoutedEventArgs e)
        {
            year.Items.Clear();
            for (int i = 2018; i <= DateTime.Now.Year; i++)
            {
                year.Items.Add(i);
            }
            year.SelectedItem = DateTime.Today.Year.ToString();
        }

        private void month_Loaded(object sender, RoutedEventArgs e)
        {
            month.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                month.Items.Add(i);
            }
            month.SelectedItem = DateTime.Today.Month.ToString();
        }

        private void day_Loaded(object sender, RoutedEventArgs e)
        {
            Day.SelectedDate = DateTime.Today;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ScrollViewer_PreviewMouseWheel_1(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
