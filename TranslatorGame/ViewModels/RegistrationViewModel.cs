using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TranslatorGame.Services;
using TranslatorGame.Views.Pages;
using Wpf.Ui.Mvvm.Contracts;

namespace TranslatorGame.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly LanguageGameService _languageGameService;
        private readonly INavigationService _navigationService;

        #region Свойства
        [ObservableProperty]
        private string _newLogin;
        [ObservableProperty]
        private string _newPassword;
        [ObservableProperty]
        private string _newPasswordCheck;
        [ObservableProperty]
        private string _loginIsExist;
        [ObservableProperty]
        private string _passwordsIsNotMatch;
        [ObservableProperty]
        private string _userAdded;
        #endregion

        public RegistrationViewModel(INavigationService navigationService,
           LanguageGameService languageGameService)
        {
            _languageGameService = languageGameService;
            _navigationService = navigationService;
        }

        #region Комманды
        [RelayCommand]
        private async Task CreateAccount()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewLogin) || string.IsNullOrWhiteSpace(NewPassword)
                    || string.IsNullOrWhiteSpace(NewPasswordCheck))
                {
                    UserAdded = "Заполните все поля";
                }
                else if (_languageGameService.CheckPlayerExists(NewLogin))
                {
                    UserAdded = string.Empty;
                    LoginIsExist = "Пользователь с таким логином уже существует";
                }
                else if (NewPassword != NewPasswordCheck)
                {
                    UserAdded = string.Empty;
                    PasswordsIsNotMatch = "Пароли не совпадают";
                }
                else
                {
                    await _languageGameService.AddNewPlayer(NewLogin, NewPassword);
                    PasswordsIsNotMatch = string.Empty;
                    UserAdded = "Пользователь добавлен";
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    Clear();
                    _navigationService.Navigate(typeof(AutorizationPage));
                }
            }
            catch (ArgumentException ex)
            {
                PasswordsIsNotMatch = ex.Message; 
            }
        }
        [RelayCommand]
        private void BackToAutorization()
        {
            Clear(); 
            _navigationService.Navigate(typeof(AutorizationPage));
        }
        #endregion

        private void Clear()
        {
            NewLogin = string.Empty;
            NewPassword = string.Empty;
            NewPasswordCheck = string.Empty;
            UserAdded = string.Empty;
            LoginIsExist = string.Empty;
            PasswordsIsNotMatch = string.Empty;
        }
    }
}
