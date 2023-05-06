using TranslatorGame.Data;
using Wpf.Ui.Common.Interfaces;

namespace TranslatorGame.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class AutorizationPage : INavigableView<ViewModels.AutorizationViewModel>
    {
        public ViewModels.AutorizationViewModel ViewModel
        {
            get;
        }

        public AutorizationPage(ViewModels.AutorizationViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();         
        }
    }
}