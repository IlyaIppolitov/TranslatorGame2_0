using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslatorGame.Data;
using TranslatorGame.Models;
using TranslatorGame.Models.Entities;
using TranslatorGame.Models.ValueObjects;

namespace TranslatorGame.Services
{
    public class LanguageGameService 
    {
        private GameDbContext _gameDbContext;
        public LanguageGameService(GameDbContext gameDbContext)
        {
            _gameDbContext = gameDbContext ?? throw new ArgumentNullException(nameof(gameDbContext));
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var listCat = await _gameDbContext.Categories.ToListAsync();

            if (listCat is null)
                throw new ArgumentNullException($"Нет подключения к базе данных!{nameof(listCat)}") ;

            if (listCat.Count < 3)
                throw new ArgumentOutOfRangeException($"Not enough categories for this UI!{nameof(listCat)}");

            return listCat;
        }

        // Получить слова по категории
        public async Task<List<Word>> GetWordByCategoryAsync(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new NullReferenceException(nameof(categoryName));
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentNullException(nameof(categoryName));
            var words = await _gameDbContext.Words
                .Where(word => word.Category!.Name == categoryName)
                .ToListAsync();

            words.ShuffleMe();
            return words;
        }

        // Добавить нового игрока
        public async Task AddNewPlayer(string login, string password)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(password)) throw new NullReferenceException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            if (CheckPlayerExists(login))
            {
                return;
            }

            var player = new Player()
            {
                Id = Guid.NewGuid(),
                Login = login,
                Password = new Password(password)
            };

            await _gameDbContext.Players.AddAsync(player);
            await _gameDbContext.SaveChangesAsync();
        }

        // Проверить существование игрока
        public bool CheckPlayerExists(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            return _gameDbContext.Players.Any(player => player.Login == login);
        }

        // Получить плохие слова игрока
        public async Task<List<Word>> GetPlayerWords(string login, string categoryName)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(categoryName)) 
                throw new NullReferenceException(nameof(categoryName));
            if (string.IsNullOrWhiteSpace(categoryName)) 
                throw new ArgumentNullException(nameof(categoryName));

            var player = await _gameDbContext.Players
                .Include(player => player.Words!)
                    .ThenInclude(word => word.Category)
                .Where(player => player.Login == login)
                .FirstAsync();
            var words = player.Words!
                .Where(word => word.Category!.Name == categoryName)
                .ToList();

            if (words == null)
                return new List<Word>();

            words.ShuffleMe();
            return words;
        }

        //добавить игрока
        public async Task AddWordToPlayerAsync(string login, Word badWord)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            if (badWord is null) throw new NullReferenceException(nameof(badWord));

            var player = await _gameDbContext.Players
                .Include(player => player.Words)
                .Where(player => player.Login == login).FirstAsync();
            if (player.Words!.Where(word => word.Id == badWord.Id).Count() == 0)
            {
                player.Words!.Add(badWord);
                await _gameDbContext.SaveChangesAsync();
            }
        }

        // Получить плохие слова игрока
        public async Task<Word> GetWordById(Guid guid)
        {
            var word = await _gameDbContext.Words
                .Where(word => word.Id == guid)
                .FirstOrDefaultAsync();
            return word!;
        }

        //получить игрока по логину
        public async Task<Player> GetPlayerAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            List<Player> players = 
                await _gameDbContext.Players
                .Where(p => p.Login == login)
                .ToListAsync();
            return players.First();
        }

        //удалить слова у игрока
        public async Task<bool> DeleteWordFromPlayerAsync(string login, Word badWord)
        {
            if (string.IsNullOrEmpty(login)) throw new NullReferenceException(nameof(login));
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login));
            if (badWord is null) throw new NullReferenceException(nameof(badWord));

            var player = await _gameDbContext.Players
                .Include(player => player.Words)
                .Where(player => player.Login == login)
                .FirstAsync();
            if (player.Words!.Any(word => word.Id == badWord.Id))
            {
                player.Words!.Remove(badWord);
                await _gameDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
