using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Domain.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date")]
    public DateOnly EndDate { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public int? FacultyProfileId { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }

    public ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}