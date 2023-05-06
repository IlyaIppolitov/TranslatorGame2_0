using Microsoft.EntityFrameworkCore;
using TranslatorGame.Models;
using TranslatorGame.Models.Entities;

namespace TranslatorGame.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) {}
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Word> Words => Set<Word>();
        public DbSet<Category> Categories => Set<Category>();
    }
}
