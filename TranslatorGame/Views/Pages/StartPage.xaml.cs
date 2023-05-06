using Wpf.Ui.Common.Interfaces;

namespace TranslatorGame.Views.Pages
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : INavigableView<ViewModels.StartViewModel>
    {
        public ViewModels.StartViewModel ViewModel
        {
            get;
        }

        public StartPage(ViewModels.StartViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();         
        }
    }
}