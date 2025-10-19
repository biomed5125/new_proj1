using System.Windows;
using HMS.CommBench.ViewModels;

namespace HMS.CommBench.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
