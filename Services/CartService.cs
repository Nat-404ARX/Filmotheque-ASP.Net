using Filmothèque.Extensions;
using Filmothèque.Models;

namespace Filmothèque.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartSession";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CartItem> GetCart()
        {
            return Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        public void AddToCart(Film film, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.FilmId == film.Id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    FilmId = film.Id,
                    Title = film.Title,
                    Price = film.Price,
                    Quantity = Math.Min(quantity, film.StockQuantity),
                    ImageUrl = film.ImageUrl
                });
            }
            else
            {
                item.Quantity = Math.Min(item.Quantity + quantity, film.StockQuantity);
            }

            Session.SetObjectAsJson(CartSessionKey, cart);
        }

        public void RemoveFromCart(int filmId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.FilmId == filmId);
            Session.SetObjectAsJson(CartSessionKey, cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public decimal GetTotal()
        {
            return GetCart().Sum(c => c.Total);
        }
    }
}