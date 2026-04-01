using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Domain.Models;

public class Branch
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Branch name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}