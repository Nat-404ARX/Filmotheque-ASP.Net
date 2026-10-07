using Microsoft.AspNetCore.Mvc;
using Filmothèque.Services;

namespace Filmothèque.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IFilmService _filmService;

        public CartController(ICartService cartService, IFilmService filmService)
        {
            _cartService = cartService;
            _filmService = filmService;
        }

        public IActionResult Index()
        {
            var items = _cartService.GetCart();
            ViewBag.Total = _cartService.GetTotal();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int filmId)
        {
            var film = await _filmService.GetFilmByIdAsync(filmId);
            if (film != null && film.StockQuantity > 0)
            {
                _cartService.AddToCart(film);
                TempData["SuccessMessage"] = $"« {film.Title} » a été ajouté à votre panier.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ce film n'est plus en stock.";
            }

            return RedirectToAction("Index", "Films");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int filmId)
        {
            _cartService.RemoveFromCart(filmId);
            return RedirectToAction(nameof(Index));
        }
    }
}