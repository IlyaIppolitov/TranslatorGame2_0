namespace TranslatorGame.Views.Pages
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage
    {
        public ViewModels.RegistrationViewModel ViewModel
        {
            get;
        }

        public RegistrationPage(ViewModels.RegistrationViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();    
        }
    }
}