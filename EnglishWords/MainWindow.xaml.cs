using EnglishWords.ViewModels;
using System;
using System.Windows;

namespace EnglishWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new ApplicationViewModel();
            DataContext = _vm;
        }
        protected override void OnClosed(EventArgs e)
        {
            _vm.StopTimer();
            base.OnClosed(e);
            Environment.Exit(0);
        }
    }
}
