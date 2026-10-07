namespace Filmothèque.Services
{
    public interface ITmdbService
    {
        Task<int> ImportPopularMoviesAsync(int pageCount = 1);
    }
}