using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Filmothèque.Models;
using Filmothèque.Services;

namespace Filmothèque.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmService _filmService;

        public FilmsController(IFilmService filmService)
        {
            _filmService = filmService;
        }

        // GET: Films (Barre de recherche + Filtrage par genre)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchString, GenreFilm? genre)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentGenre"] = genre;

            var films = await _filmService.GetAllFilmsAsync(searchString, genre) ?? new List<Film>();

            return View(films);
        }

        // GET: Films/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var film = await _filmService.GetFilmByIdAsync(id.Value);
            if (film == null) return NotFound();

            return View(film);
        }

        // GET: Films/Create (Admin uniquement)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Films/Create (Admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Rating,Price,StockQuantity,ImageUrl,Description")] Film film)
        {
            if (ModelState.IsValid)
            {
                await _filmService.CreateFilmAsync(film);
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Edit/5 (Admin uniquement)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var film = await _filmService.GetFilmByIdAsync(id.Value);
            if (film == null) return NotFound();

            return View(film);
        }

        // POST: Films/Edit/5 (Admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Rating,Price,StockQuantity,ImageUrl,Description")] Film film)
        {
            if (id != film.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _filmService.UpdateFilmAsync(film);
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Delete/5 (Admin uniquement)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var film = await _filmService.GetFilmByIdAsync(id.Value);
            if (film == null) return NotFound();

            return View(film);
        }

        // POST: Films/Delete/5 (Admin uniquement)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _filmService.DeleteFilmAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Films/ImportTmdb (Admin uniquement)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportTmdb([FromServices] ITmdbService tmdbService, int pages = 1)
        {
            try
            {
                int count = await tmdbService.ImportPopularMoviesAsync(pages);
                TempData["SuccessMessage"] = $"{count} film(s) importé(s) avec succès depuis TMDB !";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de l'import : {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Films/DeleteAll (Admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAll()
        {
            await _filmService.DeleteAllFilmsAsync();
            TempData["SuccessMessage"] = "Tous les films ont été supprimés de la base de données.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Films/ManageStock (Admin uniquement)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageStock()
        {
            var films = await _filmService.GetAllFilmsAsync(null, null);
            return View(films);
        }

        // POST: Films/UpdateStock (Admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int id, int stockQuantity)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film != null)
            {
                film.StockQuantity = Math.Max(0, stockQuantity);
                await _filmService.UpdateFilmAsync(film);
                TempData["SuccessMessage"] = $"Le stock de « {film.Title} » a été mis à jour ({film.StockQuantity} unité(s)).";
            }
            return RedirectToAction(nameof(ManageStock));
        }
    }
}