using System.ComponentModel.DataAnnotations;

public class CreateMovieDto
{
    [Required]
    public IFormFile Poster { get; set; }

    [Required, StringLength(100, MinimumLength = 2)]
    public string Title { get; set; }

    [Required, StringLength(500)]
    public string Description { get; set; }
    [Required, DataType(DataType.Date)]
    [NotInFuture(ErrorMessage = "Release date cannot be in the future")]
    public DateTime ReleaseDate { get; set; }


    [Range(1, double.MaxValue)]
    public double Budget { get; set; }
    [Required, StringLength(100)]
    public string Category { get; set; }

}
