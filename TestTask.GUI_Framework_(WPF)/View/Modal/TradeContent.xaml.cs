using System.Windows.Controls;
using TestTask.GUI_Framework__WPF_.ViewModel.Modal;

namespace TestTask.GUI_Framework__WPF_.View.Modal
{
    public partial class TradeContent : UserControl , IModal
    {
        public TradeContent()
        {
            InitializeComponent();
            DataContext = new TradeContentViewModel();
        }
    }
}
