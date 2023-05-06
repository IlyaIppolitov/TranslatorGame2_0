using Wpf.Ui.Common.Interfaces;

namespace TranslatorGame.Views.Pages
{
    /// <summary>
    /// Interaction logic for NewGamePage.xaml
    /// </summary>
    public partial class NewGamePage : INavigableView<ViewModels.NewGameViewModel>
    {
        public ViewModels.NewGameViewModel ViewModel
        {
            get;
        }

        public NewGamePage(ViewModels.NewGameViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

    }
}