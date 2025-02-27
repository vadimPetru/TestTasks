﻿using ConnectorTest;
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
using TestTask.GUI_Framework__WPF_.ViewModel;

namespace TestTask.GUI_Framework__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ITestConnector connector)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(connector);
        }
    }
}