using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TranslatorGame.Models.Entities;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;
using TranslatorGame.Views.Pages;
using TranslatorGame.Services;
using CommunityToolkit.Mvvm.Messaging;
using TranslatorGame.Messages;
using System.Windows;

namespace TranslatorGame.ViewModels
{
    public partial class AutorizationViewModel : ObservableObject
    {
        private readonly LanguageGameService _languageGameService; 
        private readonly INavigationService _navigationService;

        #region Свойства
        [ObservableProperty]
        private string _login;
        [ObservableProperty]
        private string _password; 
         [ObservableProperty]
        private string _userIsNotFound;
        #endregion

        public AutorizationViewModel(INavigationService navigationService, 
            LanguageGameService languageGameService)
        {
            _languageGameService = languageGameService;
            _navigationService = navigationService;
            //RegisterMessage(); 
        }

        #region Комманды
        [RelayCommand]
        private async Task ComeInAccount()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Login))
            {
                UserIsNotFound = "Неверный логин и/или пароль";
            }
            else if (!_languageGameService.CheckPlayerExists(Login))
            {
                UserIsNotFound = "Неверный логин и/или пароль";
            }
            else
            {
                RegisterMessage();
                Player player = await _languageGameService.GetPlayerAsync(Login);
                if (player.Password!.ToString() == Password)
                {                
                    _navigationService.Navigate(typeof(StartPage)); 
                }
                else
                {
                    UserIsNotFound = "Неверный логин и/или пароль";
                }
            }
        }
        [RelayCommand]
        private void RegistrationAccount()
        {
            _navigationService.Navigate(typeof(RegistrationPage));
        }
        #endregion
        private void Clear()
        {
            Login = string.Empty;
            Password = string.Empty;
        }
        private void RegisterMessage()
        {
            WeakReferenceMessenger.Default.Register<LoginRequestMessage>(this, (r, m) =>
            {
                m.Reply(Login);
            });
        }
    }
}

