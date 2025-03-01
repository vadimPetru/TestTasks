using System.Windows.Controls;
using TestTask.GUI_Framework__WPF_.ViewModel;

namespace TestTask.GUI_Framework__WPF_.View.Windows
{
   
    public partial class ModelWindow : UserControl
    {
        public ModelWindow()
        {
            InitializeComponent();
           
            DataContext = new ModelWindowViewModel();
        }
    }
}
