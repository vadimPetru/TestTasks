using System.Windows.Controls;
using TestTask.GUI_Framework__WPF_.ViewModel.Modal;

namespace TestTask.GUI_Framework__WPF_.View.Modal
{

    public partial class CandleContent : UserControl , IModal
    {
        public CandleContent()
        {
            InitializeComponent();
            DataContext = new CandleContentViewModel();
        }
    }
}
