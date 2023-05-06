using Wpf.Ui.Common.Interfaces;

namespace TranslatorGame.Views.Pages
{
    /// <summary>
    /// Interaction logic for ChoiceCategoryGamePage.xaml
    /// </summary>
    public partial class CategoryGamePage : INavigableView<ViewModels.GategoryGameViewModel>
    {
        public ViewModels.GategoryGameViewModel ViewModel
        {
            get;
        }

        public CategoryGamePage(ViewModels.GategoryGameViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}