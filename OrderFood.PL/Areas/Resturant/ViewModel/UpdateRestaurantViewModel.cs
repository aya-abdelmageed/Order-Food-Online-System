using System.ComponentModel.DataAnnotations;

public class UpdateRestaurantViewModel
{
    public IFormFile? ImageFile { get; set; }

    public string? Logo { get; set; }

    public int Id { get; set; }

    public string OwnerId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(200)]
    public string Address { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(550)]
    public string Description { get; set; }

    [Required(ErrorMessage = "HotLine is required")]
    [RegularExpression(@"^\d{3,15}$", ErrorMessage = "HotLine must be between 3 and 15 digits")]
    public string HotLine { get; set; }

    public double? Long { get; set; }
    public double? Lat { get; set; }
}
