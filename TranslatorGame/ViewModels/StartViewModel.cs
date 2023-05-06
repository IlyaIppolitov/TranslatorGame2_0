using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TranslatorGame.Messanges;
using TranslatorGame.Models;
using TranslatorGame.Views.Pages;
using Wpf.Ui.Mvvm.Contracts;

namespace TranslatorGame.ViewModels
{
    public partial class StartViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        #region Свойства
        [ObservableProperty]
        private bool _englisgChecked = true;
        [ObservableProperty]
        private bool _germangChecked = false;
        [ObservableProperty]
        private LanguageOptions _englishLanguage = LanguageOptions.English;
        [ObservableProperty]
        private LanguageOptions _germanLanguage = LanguageOptions.German;
        [ObservableProperty]
        private LanguageOptions _language = LanguageOptions.English;
        #endregion

        public StartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            RegisterMessage();
        }

        #region Комманды
        [RelayCommand]
        private void BackToAutorizationPage() =>
            _navigationService.Navigate(typeof(AutorizationPage));      
        [RelayCommand]
        private void StartNewGame() => _navigationService.Navigate(typeof(CategoryGamePage));
        [RelayCommand]
        private void SetEnglishLanguage()
        {
            if (EnglisgChecked)
            {
                GermangChecked = false;
                Language = Models.LanguageOptions.English;
            }
            else
            {
                GermangChecked = true;
                Language = Models.LanguageOptions.German;
            }
        }
        [RelayCommand]
        private void SetGermanLanguage()
        {
            if (GermangChecked)
            {
                EnglisgChecked = false;
                Language = Models.LanguageOptions.German;
            }
            else
            {
                EnglisgChecked = true;
                Language = Models.LanguageOptions.German;
            }
        }
        #endregion

        private void RegisterMessage()
        {
            WeakReferenceMessenger.Default.Register<LanguageRequestMessage>(this, (r, m) =>
            {
                m.Reply(Language);
            });
        }
    }
}
