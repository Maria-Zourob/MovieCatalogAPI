//MoviesController
using Asp_FinalProject.Models;
using Asp_FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.ComponentModel;

namespace MovieCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAll()
        {
            var movies = await _movieService.GetAllAsync();
            if (movies == null || movies.Count == 0)
                return NotFound("No movies found or an error occurred.");
            return Ok(movies);
        }

        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Movie>>> Search([FromQuery] string query)
        {
            var movies = await _movieService.SearchAsync(query);
            if (movies == null || movies.Count == 0)
                return NotFound("No matching movies found.");
            return Ok(movies);
        }

        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("bycategory")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required.");

            var movies = await _movieService.GetByCategoryAsync(category);
            if (movies == null || movies.Count == 0)
                return NotFound($"No movies found in category '{category}'.");

            return Ok(movies);
        }
        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _movieService.GetDistinctCategoriesAsync();
            if (categories == null || categories.Count == 0)
                return NotFound("No categories found.");

            return Ok(categories);
        }
        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetLatest()
        {
            var movies = await _movieService.GetLatestMoviesAsync();
            if (movies == null || movies.Count == 0)
                return NotFound("No latest movies found.");

            return Ok(movies);
        }

        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetById(int id)
        {
            //if (!User.IsInRole("Admin"))
            //    return BadRequest("Only Admin users are allowed to perform this action.");
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound("Movie not found.");
            return Ok(movie);
        }

        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetMovieCount([FromQuery] string? category = null)
        {
            int count = await _movieService.CountMoviesAsync(category);
            return Ok(count);
        }
        [Authorize(Roles = "Admin,Reader")]
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetByDateRange(DateTime start, DateTime end)
        {
            var movies = await _movieService.GetByDateRangeAsync(start, end);
            if (movies == null || movies.Count == 0)
                return NotFound("No movies found in the specified date range.");

            return Ok(movies);
        }

        /// ///////////////
 

        ////Engineer Zakaria, I made these as an addition from me, but I 
        ////    made a comment with the code on them because they want to download 
        ////    the package and I don’t want the file to get larger

        //[Authorize(Roles = "Admin,Reader")]
        //[HttpGet("export/excel")]
        //public async Task<IActionResult> ExportToExcel()
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    var movies = await _movieService.GetAllAsync();
        //    if (movies == null || movies.Count == 0)
        //        return NotFound("No movies found.");

        //    using var package = new OfficeOpenXml.ExcelPackage();
        //    var worksheet = package.Workbook.Worksheets.Add("Movies");


        //    worksheet.Cells[1, 1].Value = "Title";
        //    worksheet.Cells[1, 2].Value = "Description";
        //    worksheet.Cells[1, 3].Value = "Category";
        //    worksheet.Cells[1, 4].Value = "Budget";
        //    worksheet.Cells[1, 5].Value = "Release Date";


        //    for (int i = 0; i < movies.Count; i++)
        //    {
        //        var movie = movies[i];
        //        worksheet.Cells[i + 2, 1].Value = movie.Title;
        //        worksheet.Cells[i + 2, 2].Value = movie.Description;
        //        worksheet.Cells[i + 2, 3].Value = movie.Category;
        //        worksheet.Cells[i + 2, 4].Value = movie.Budget;
        //        worksheet.Cells[i + 2, 5].Value = movie.ReleaseDate.ToString("yyyy-MM-dd");
        //    }


        //    worksheet.Column(5).Style.Numberformat.Format = "yyyy-mm-dd";

        //    var stream = new MemoryStream(package.GetAsByteArray());

        //    return File(stream.ToArray(),
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                "movies_export.xlsx");
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost("import/excel")]
        //public async Task<IActionResult> ImportFromExcel(IFormFile file)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var movies = new List<Movie>();
        //    int skipped = 0;

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);
        //    using var package = new OfficeOpenXml.ExcelPackage(stream);
        //    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //    if (worksheet == null)
        //        return BadRequest("Invalid Excel file. No worksheet found.");

        //    var expectedHeaders = new[] { "Title", "Description", "Category", "Budget", "ReleaseDate" };
        //    for (int i = 0; i < expectedHeaders.Length; i++)
        //    {
        //        var header = worksheet.Cells[1, i + 1].Text?.Trim();
        //        if (!string.Equals(header, expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
        //        {
        //            return BadRequest($"Invalid header in column {i + 1}. Expected: '{expectedHeaders[i]}', Found: '{header}'");
        //        }
        //    }

        //    int rowCount = worksheet.Dimension.Rows;

        //    for (int row = 2; row <= rowCount; row++)
        //    {
        //        try
        //        {
        //            string title = worksheet.Cells[row, 1].Text.Trim();
        //            string desc = worksheet.Cells[row, 2].Text.Trim();
        //            string cat = worksheet.Cells[row, 3].Text.Trim();
        //            string budgetStr = worksheet.Cells[row, 4].Text.Trim();
        //            string dateStr = worksheet.Cells[row, 5].Text.Trim();

        //            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(desc) || string.IsNullOrWhiteSpace(cat))
        //            {
        //                skipped++;
        //                continue;
        //            }

        //            if (!double.TryParse(budgetStr, out double budget) || budget <= 0)
        //            {
        //                skipped++;
        //                continue;
        //            }

        //            if (!DateTime.TryParse(dateStr, out DateTime releaseDate) || releaseDate > DateTime.Today)
        //            {
        //                skipped++;
        //                continue;
        //            }

        //            movies.Add(new Movie
        //            {
        //                Title = title,
        //                Description = desc,
        //                Category = cat,
        //                Budget = budget,
        //                ReleaseDate = releaseDate,
        //                PosterUrl = "/posters/no-poster.jpg" // صورة افتراضية
        //            });
        //        }
        //        catch
        //        {
        //            skipped++;
        //            continue;
        //        }
        //    }

        //    foreach (var movie in movies)
        //    {
        //        await _movieService.AddAsync(movie);
        //    }

        //    return Ok($"{movies.Count} movies imported successfully. Skipped: {skipped} rows.");
        //}


        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<Movie>> Create([FromForm] CreateMovieDto model)
        {
            //if (!User.IsInRole("Admin"))
            //    return BadRequest("Only Admin users are allowed to perform this action.");
            if (model.Poster == null || model.Poster.Length == 0)
                return BadRequest("Poster image is required.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(model.Poster.FileName).ToLower();
            if (model.Poster.Length > 2 * 1024 * 1024)
                return BadRequest("Image size must be less than 2MB.");


            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");

            var fileName = Guid.NewGuid().ToString() + ext;
            var filePath = Path.Combine("wwwroot", "posters", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Poster.CopyToAsync(stream);
            }

            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                ReleaseDate = model.ReleaseDate,
                Budget = model.Budget,
                PosterUrl = $"/posters/{fileName}",
                Category = model.Category

            };

            var added = await _movieService.AddAsync(movie);
            if (added == null)
                return StatusCode(500, "Failed to add movie.");

            return CreatedAtAction(nameof(GetById), new { id = added.Id }, added);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateMovieDto model)
        {
            //if (!User.IsInRole("Admin"))
            //    return BadRequest("Only Admin users are allowed to perform this action.");
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound("Movie not found.");

            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Budget = model.Budget;
            movie.Category = model.Category;


            if (model.Poster != null && model.Poster.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(model.Poster.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Only image files are allowed.");
                if (model.Poster.Length > 2 * 1024 * 1024) // حجم أكبر من 2MB
                    return BadRequest("Image size must be less than 2MB.");

                if (!string.IsNullOrEmpty(movie.PosterUrl))
                {
                    var oldImagePath = Path.Combine("wwwroot", movie.PosterUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                var fileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine("wwwroot", "posters", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Poster.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/posters/{fileName}";
            }

            var (success, msg) = await _movieService.UpdateAsync(id, movie);
            if (!success)
                return StatusCode(500, msg);

            return Ok(msg);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //if (!User.IsInRole("Admin"))
            //    return BadRequest("Only Admin users are allowed to perform this action.");
            var existing = await _movieService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Movie not found.");

            
            if (!string.IsNullOrEmpty(existing.PosterUrl))
            {
                var imagePath = Path.Combine("wwwroot", existing.PosterUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            var (success, msg) = await _movieService.DeleteAsync(id);
            if (!success)
                return StatusCode(500, msg);

            return Ok(msg);
        }
    
        [Authorize(Roles = "Admin")]
        [HttpPost("delete-bulk")]
        public async Task<ActionResult> BulkDelete([FromBody] List<int> ids)
        {
            var (success, message) = await _movieService.DeleteMultipleAsync(ids);
            if (!success) return StatusCode(500, message);

            return Ok(message);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("suggest-category")]
        public ActionResult<string> SuggestCategory([FromBody] string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return BadRequest("Input is required.");

            var suggestion = _movieService.SuggestCategoryFromText(input);
            return Ok(suggestion);
        }

    }
}
