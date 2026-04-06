using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Models;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers;

[Authorize]
public class ExamController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ExamController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> ByCourse(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Exams)
                .ThenInclude(e => e.Results)
                    .ThenInclude(r => r.StudentProfile)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null) return NotFound();

        if (User.IsInRole("Faculty"))
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user!.Id);
            if (course.FacultyProfileId != faculty!.Id) return Forbid();
        }

        ViewBag.CourseName = course.Name;
        ViewBag.CourseId = courseId;
        return View(course.Exams.ToList());
    }

    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Create(int courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        ViewBag.CourseName = course.Name;
        ViewBag.CourseId = courseId;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Faculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Exam model)
    {
        ModelState.Remove("Course");
        ModelState.Remove("Results");

        if (!ModelState.IsValid)
        {
            ViewBag.CourseName = (await _context.Courses.FindAsync(model.CourseId))?.Name;
            ViewBag.CourseId = model.CourseId;
            return View(model);
        }

        _context.Exams.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ByCourse), new { courseId = model.CourseId });
    }

    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Results(int examId)
    {
        var exam = await _context.Exams
            .Include(e => e.Course)
            .Include(e => e.Results)
                .ThenInclude(r => r.StudentProfile)
            .FirstOrDefaultAsync(e => e.Id == examId);

        if (exam == null) return NotFound();

        if (User.IsInRole("Faculty"))
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user!.Id);
            if (exam.Course.FacultyProfileId != faculty!.Id) return Forbid();
        }

        var enrolled = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.CourseId == exam.CourseId)
            .ToListAsync();

        ViewBag.Enrolled = enrolled;
        return View(exam);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Faculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveResult(int examId, int studentProfileId, int score, string? grade)
    {
        var existing = await _context.ExamResults
            .FirstOrDefaultAsync(r => r.ExamId == examId
                                   && r.StudentProfileId == studentProfileId);
        if (existing != null)
        {
            existing.Score = score;
            existing.Grade = grade;
        }
        else
        {
            _context.ExamResults.Add(new ExamResult
            {
                ExamId = examId,
                StudentProfileId = studentProfileId,
                Score = score,
                Grade = grade
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Results), new { examId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleRelease(int examId)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        exam.ResultsReleased = !exam.ResultsReleased;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Results), new { examId });
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyResults()
    {
        var user = await _userManager.GetUserAsync(User);
        var student = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.IdentityUserId == user!.Id);

        if (student == null) return NotFound();

        var results = await _context.ExamResults
            .Include(r => r.Exam)
                .ThenInclude(e => e.Course)
            .Where(r => r.StudentProfileId == student.Id
                     && r.Exam.ResultsReleased == true)
            .ToListAsync();

        return View(results);
    }
}
