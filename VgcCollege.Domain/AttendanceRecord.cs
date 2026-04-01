using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Domain.Models;

public class AttendanceRecord
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Week Number")]
    public int WeekNumber { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    public bool Present { get; set; }

    public int CourseEnrolmentId { get; set; }
    public CourseEnrolment CourseEnrolment { get; set; } = null!;
}