using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Filmothèque.Services;

namespace Filmothèque.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IFilmService _filmService;

        public CheckoutController(ICartService cartService, IFilmService filmService)
        {
            _cartService = cartService;
            _filmService = filmService;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            var cartItems = _cartService.GetCart();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Votre panier est vide.";
                return RedirectToAction("Index", "Cart");
            }

            var domain = $"{Request.Scheme}://{Request.Host}";

            var lineItems = cartItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // Stripe utilise les centimes
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Title,
                    },
                },
                Quantity = item.Quantity,
            }).ToList();

            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{domain}/Checkout/Success",
                CancelUrl = $"{domain}/Cart/Index",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success()
        {
            var cartItems = _cartService.GetCart();

            // Décrémentation automatique des stocks pour chaque film achete
            foreach (var item in cartItems)
            {
                var film = await _filmService.GetFilmByIdAsync(item.FilmId);
                if (film != null)
                {
                    film.StockQuantity = Math.Max(0, film.StockQuantity - item.Quantity);
                    await _filmService.UpdateFilmAsync(film);
                }
            }

            // Vider le panier
            _cartService.ClearCart();

            return View();
        }
    }
}