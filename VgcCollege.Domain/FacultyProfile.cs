using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Domain.Models;

public class FacultyProfile
{
    public int Id { get; set; }

    public string IdentityUserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}