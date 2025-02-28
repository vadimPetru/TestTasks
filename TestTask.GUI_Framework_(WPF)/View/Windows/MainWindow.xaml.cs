using ConnectorTest;
using System.Windows;
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