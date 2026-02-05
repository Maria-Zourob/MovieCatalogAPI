//MovieServices
using Asp_FinalProject.Data;
using Asp_FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace Asp_FinalProject.Services
{
    public class MovieService
    {
        private readonly AppDbContext _context;

        public MovieService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>?> GetAllAsync()
        {
            try
            {
                return await _context.Movies.AsNoTracking().ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Movie>?> SearchAsync(string query)
        {
            try
            {
                return await _context.Movies
                    .AsNoTracking()
                    .Where(m => m.Title.Contains(query) || m.Description.Contains(query))
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Movie?> AddAsync(Movie movie)
        {
            try
            {
                await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();
                return movie;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool success, string message)> UpdateAsync(int id, Movie movie)
        {
            try
            {
                if (id != movie.Id)
                    return (false, "Movie ID does not match.");

                _context.Entry(movie).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return (true, "Movie updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "Movie not found or concurrency error.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            try
            {
                var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                if (movie == null)
                    return (false, "Movie not found.");

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return (true, "Movie deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting the movie: {ex.Message}");
            }
        }
        public async Task<List<Movie>?> GetByCategoryAsync(string category)
        {
            try
            {
                return await _context.Movies
                    .AsNoTracking()
                    .Where(m => m.Category.ToLower() == category.ToLower())
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<int> CountMoviesAsync(string? category = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(category))
                    return await _context.Movies.CountAsync(m => m.Category.ToLower() == category.ToLower());

                return await _context.Movies.CountAsync();
            }
            catch
            {
                return 0;
            }
        }
        public async Task<List<Movie>?> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            try
            {
                return await _context.Movies
                    .AsNoTracking()
                    .Where(m => m.ReleaseDate >= start && m.ReleaseDate <= end)
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<string>?> GetDistinctCategoriesAsync()
        {
            try
            {
                return await _context.Movies
                    .AsNoTracking()
                    .Select(m => m.Category)
                    .Distinct()
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<Movie>?> GetLatestMoviesAsync()
        {
            try
            {
                return await _context.Movies
                    .AsNoTracking()
                    .OrderByDescending(m => m.ReleaseDate)
                    .Take(10)
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<(bool success, string message)> DeleteMultipleAsync(List<int> ids)
        {
            try
            {
                var movies = await _context.Movies
                    .Where(m => ids.Contains(m.Id))
                    .ToListAsync();

                if (movies.Count == 0)
                    return (false, "No matching movies found.");

                foreach (var movie in movies)
                {
                    if (!string.IsNullOrEmpty(movie.PosterUrl))
                    {
                        var imagePath = Path.Combine("wwwroot", movie.PosterUrl.TrimStart('/'));
                        if (File.Exists(imagePath))
                            File.Delete(imagePath);
                    }
                }

                _context.Movies.RemoveRange(movies);
                await _context.SaveChangesAsync();

                return (true, $"{movies.Count} movies deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting movies: {ex.Message}");
            }
        }
        public string SuggestCategoryFromText(string input)
        {
            input = input.ToLower();

            if (input.Contains("space") || input.Contains("alien") || input.Contains("robot"))
                return "Sci-Fi";

            if (input.Contains("love") || input.Contains("romance") || input.Contains("heart"))
                return "Romance";

            if (input.Contains("war") || input.Contains("battle") || input.Contains("soldier"))
                return "Action";

            if (input.Contains("funny") || input.Contains("laugh") || input.Contains("joke"))
                return "Comedy";

            if (input.Contains("ghost") || input.Contains("haunted") || input.Contains("scary"))
                return "Horror";

            return "General";
        }

    }
}
