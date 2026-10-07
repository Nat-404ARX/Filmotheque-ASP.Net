using Filmothèque.Data;
using Filmothèque.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmothèque.Services
{
    public class FilmService : IFilmService
    {
        private readonly ApplicationDbContext _context;

        public FilmService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Film>> GetAllFilmsAsync(string? searchString, GenreFilm? genre)
        {
            var query = _context.Films.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(f => f.Title.ToLower().Contains(searchString.ToLower()));
            }

            if (genre.HasValue)
            {
                query = query.Where(f => f.Genre == genre.Value);
            }

            // ToListAsync() garantit de retourner une liste (vide si aucun résultat), jamais null
            return await query.ToListAsync() ?? new List<Film>();
        }

        public async Task<Film?> GetFilmByIdAsync(int id)
        {
            return await _context.Films.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task CreateFilmAsync(Film film)
        {
            _context.Films.Add(film);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFilmAsync(Film film)
        {
            _context.Films.Update(film);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFilmAsync(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllFilmsAsync()
        {
            _context.Films.RemoveRange(_context.Films);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> FilmExistsAsync(int id)
        {
            return await _context.Films.AnyAsync(e => e.Id == id);
        }
    }
}