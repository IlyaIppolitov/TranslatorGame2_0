using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TranslatorGame.Messages;
using TranslatorGame.Messanges;
using TranslatorGame.Models;
using TranslatorGame.Services;
using TranslatorGame.Views.Pages;
using Wpf.Ui.Mvvm.Contracts;

namespace TranslatorGame.ViewModels
{
    public partial class NewGameViewModel : ObservableObject
    {
        #region Поля
        private IEnumerator<Word> _enumerator;
        private WordProviderService _provider;
        private List<Word> _dictionaryWords;
        private List<Word> _playerWords;
        private string _playerLogin;
        private int _rightNumber;

        private LanguageOptions _language;
        private string _category;
        private readonly LanguageGameService _languageGameService;
        private readonly ImageService _imageService;
        private readonly INavigationService _navigationService;
        private string _key;
        private readonly OpenAiClient _client;
        #endregion

        #region Свойства
        [ObservableProperty]
        private string _answerFirstButton;
        [ObservableProperty]
        private string _answerSecondButton;
        [ObservableProperty]
        private string _answerThirdButton;
        [ObservableProperty]
        private string _answerFourthButton;
        [ObservableProperty]
        private BitmapImage _outputImage;
        [ObservableProperty]
        private string _gameIsDone;
        [ObservableProperty]
        private string _colorAnswer = "Pink";
        [ObservableProperty]
        private string _quessWord;
        public Word QWord { get; set; }
        public string Content { get; set; }
        #endregion

        public NewGameViewModel(INavigationService navigationService,
            LanguageGameService languageGameService)
        {
            _languageGameService = languageGameService;
            _navigationService = navigationService;

            _key = Environment.GetEnvironmentVariable("openai_api_key")
                ?? throw new InvalidOperationException(nameof(_key));

            if (_key is not null)
            {
                _client = new OpenAiClient(_key);
            }
            else
            {
                throw new InvalidOperationException("Переменная окружения openai_api_key не задана!");
            }
            _imageService = new ImageService(_client); 
        }

        #region Комманды
        [RelayCommand]
        private async Task Loaded()
        {
            var language = WeakReferenceMessenger.Default.Send(new LanguageRequestMessage());
            _language = language.Response;

            var category = WeakReferenceMessenger.Default.Send(new CategoryRequestMessage());
            _category = category.Response;

            var login = WeakReferenceMessenger.Default.Send(new LoginRequestMessage());
            _playerLogin = login.Response; 

            _dictionaryWords = await _languageGameService.GetWordByCategoryAsync(_category);
            _playerWords = await _languageGameService.GetPlayerWords(_playerLogin, _category);

            _provider = new WordProviderService(new[]
            {
                (_dictionaryWords, 1.0),
                (_playerWords, 0.6)
            });
            _enumerator = _provider
                .Take(_dictionaryWords.Count + _playerWords.Count)
                .GetEnumerator();
            await FillAllButtons();
        }
        [RelayCommand]
        private void BackToChoiceCategory() => _navigationService.Navigate(typeof(CategoryGamePage));
        [RelayCommand]
        private async Task CheckAnswer(object parameter)
        {
            if (parameter is string answer)
            {
                if (string.IsNullOrEmpty(answer))
                    throw new NullReferenceException(nameof(answer));
                if (string.IsNullOrWhiteSpace(answer))
                    throw new ArgumentNullException(nameof(answer));
                Content = answer;
            }
            else
            { 
                //нужно выбросить исключение
            }

            if (await CheckRightButton(Content))
            {
                ColorAnswer = "Green";
            }
            else
            {
                ColorAnswer = "Red";
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            ColorAnswer = "Pink";
            await FillAllButtons();
        }
        #endregion

        private async Task FillAllButtons()
        {
            _enumerator.MoveNext();

            QWord = _enumerator.Current;

            if (QWord == null)
            {
                GameIsDone = "Молодец! Ты прошёл все слова в этой категории!";
                await Task.Delay(TimeSpan.FromSeconds(5));
                _navigationService.Navigate(typeof(CategoryGamePage));
                GameIsDone = string.Empty; 
                return; 
            }

            QuessWord = QWord.Russian!;

            Random rnd = new Random();
            //OutputImage = await _imageService.GetPictureAsync(QWord.English);

            _rightNumber = rnd.Next(4) + 1;
            PutContentToButton(_rightNumber, QWord);

            var options = GetOptionsWords(QWord, _dictionaryWords);

            int j = 0;
            for (int i = 1; i <= 4; ++i)
            {
                if (i == _rightNumber)
                {
                    continue;
                }
                PutContentToButton(i, options[j]);
                j++;
            }
        }
        void PutContentToButton(int buttonNumber, Word word)
        {
            if (word is null) throw new NullReferenceException(nameof(word));

            string wordString;
            switch (_language)
            {
                case LanguageOptions.English:
                    wordString = word.English;
                    break;

                case LanguageOptions.German:
                    wordString = word.German;
                    break;
                default:
                    throw new ArgumentNullException(nameof(_language));
            }
            switch (buttonNumber)
            {
                case 1:
                    AnswerFirstButton = wordString;
                    break;
                case 2:
                    AnswerSecondButton = wordString;
                    break;
                case 3:
                    AnswerThirdButton = wordString;
                    break;
                case 4:
                    AnswerFourthButton = wordString;
                    break;
            }
        }
        List<Word> GetOptionsWords(Word qWord, List<Word> words)
        {
            if (qWord is null) throw new NullReferenceException(nameof(qWord));
            if (words is null) throw new NullReferenceException(nameof(words));

            List<Word> options = new List<Word>();
            var countOfWords = words.Count;
            Random rnd = new Random();

            while (options.Count != 3)
            {
                var word = words[rnd.Next(countOfWords)];
                if (word.Id == qWord.Id) continue;
                var flag = false;
                foreach (var w in options)
                {
                    if (w.Id == word.Id) flag = true;
                }
                if (flag) continue;

                options.Add(word);
            }
            return options;
        }
        private async Task<bool> CheckRightButton(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrEmpty(content))
                throw new NullReferenceException(nameof(content));

            if (_language == LanguageOptions.English)
            {
                if (content == QWord.English)
                {
                    return true;
                }
                else
                {
                    await _languageGameService.AddWordToPlayerAsync(_playerLogin, QWord);
                    return false;
                }
            }
            else if (_language == LanguageOptions.German)
            {
                if (content == QWord.German)
                {
                    return true;
                }
                else
                {
                    await _languageGameService.AddWordToPlayerAsync(_playerLogin, QWord);
                    return false;
                }
            }
            throw new ArgumentException(nameof(_language));
        }
    }
}
