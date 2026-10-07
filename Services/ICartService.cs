using Filmothèque.Models;

namespace Filmothèque.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(Film film, int quantity = 1);
        void RemoveFromCart(int filmId);
        void ClearCart();
        decimal GetTotal();
    }
}