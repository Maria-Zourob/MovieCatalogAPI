//Movie.cs
namespace Asp_FinalProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description can't be more than 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Poster URL is required"), Url(ErrorMessage = "Invalid URL format")]
        [StringLength(200, ErrorMessage = "Poster URL is too long")]
        [RegularExpression(@".*\.(jpg|jpeg|png|gif|webp)$", ErrorMessage = "Only .jpg, .jpeg, .png, .gif, or .webp image URLs are allowed")]

        public string PosterUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release date is required"), DataType(DataType.Date)]
        [NotInFuture(ErrorMessage = "Release date cannot be in the future")]
        public DateTime ReleaseDate { get; set; } 

        [Range(1, double.MaxValue, ErrorMessage = "Budget must be greater than 0")]
        public double Budget { get; set; }
        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category can't be longer than 100 characters")]
        public string Category { get; set; } = string.Empty;

    }

}
