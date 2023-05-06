using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TranslatorGame.Messages;
using TranslatorGame.Services;
using TranslatorGame.Views.Pages;
using Wpf.Ui.Mvvm.Contracts;

namespace TranslatorGame.ViewModels
{
    public partial class GategoryGameViewModel : ObservableObject
    {
        private LanguageGameService _languageGameService;
        private readonly INavigationService _navigationService;

        #region Свойства
        [ObservableProperty]
        private string _category;
        [ObservableProperty]
        private string _categoryMedicine;
        [ObservableProperty]
        private string _categoryAnimals;
        [ObservableProperty]
        private string _categoryIT;
        #endregion

        public GategoryGameViewModel(INavigationService navigationService,
            LanguageGameService languageGameService)
        {
            _languageGameService = languageGameService;
            _navigationService = navigationService;
            RegisterMessage();
        }

        #region Комманды
        [RelayCommand]
        private async Task Loaded()
        {
            var categories = await _languageGameService.GetCategoriesAsync();
            CategoryMedicine = categories.Select(c => c.Name).ToList()[1];
            CategoryAnimals = categories.Select(c => c.Name).ToList()[0];
            CategoryIT = categories.Select(c => c.Name).ToList()[2];
        }
        [RelayCommand]
        private void NewGame(object p)
        {     
            if (p is string category)
            {
                if (string.IsNullOrEmpty(category)) throw new NullReferenceException(nameof(category));
                if (string.IsNullOrWhiteSpace(category)) throw new ArgumentNullException(nameof(category));
                Category = category;
                _navigationService.Navigate(typeof(NewGamePage));
            }
        }
        [RelayCommand]
        private void BackToStartPage() => _navigationService.Navigate(typeof(StartPage));
        #endregion

        private void RegisterMessage()
        {
            WeakReferenceMessenger.Default.Register<CategoryRequestMessage>(this, (r, m) =>
            {
                m.Reply(Category);
            });
        }
    }
}
