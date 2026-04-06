using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Models;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers;

[Authorize]
public class AssignmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AssignmentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> ByCourse(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Assignments)
                .ThenInclude(a => a.Results)
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
        return View(course.Assignments.ToList());
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
    public async Task<IActionResult> Create(Assignment model)
    {
        ModelState.Remove("Course");
        ModelState.Remove("Results");

        if (!ModelState.IsValid)
        {
            ViewBag.CourseName = (await _context.Courses.FindAsync(model.CourseId))?.Name;
            ViewBag.CourseId = model.CourseId;
            return View(model);
        }

        _context.Assignments.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ByCourse), new { courseId = model.CourseId });
    }

    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> Results(int assignmentId)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Course)
            .Include(a => a.Results)
                .ThenInclude(r => r.StudentProfile)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null) return NotFound();

        if (User.IsInRole("Faculty"))
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user!.Id);
            if (assignment.Course.FacultyProfileId != faculty!.Id) return Forbid();
        }

        var enrolled = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.CourseId == assignment.CourseId)
            .ToListAsync();

        ViewBag.Enrolled = enrolled;
        return View(assignment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Faculty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveResult(int assignmentId, int studentProfileId, int score, string? feedback)
    {
        var existing = await _context.AssignmentResults
            .FirstOrDefaultAsync(r => r.AssignmentId == assignmentId
                                   && r.StudentProfileId == studentProfileId);
        if (existing != null)
        {
            existing.Score = score;
            existing.Feedback = feedback;
        }
        else
        {
            _context.AssignmentResults.Add(new AssignmentResult
            {
                AssignmentId = assignmentId,
                StudentProfileId = studentProfileId,
                Score = score,
                Feedback = feedback
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Results), new { assignmentId });
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyResults()
    {
        var user = await _userManager.GetUserAsync(User);
        var student = await _context.StudentProfiles
            .FirstOrDefaultAsync(s => s.IdentityUserId == user!.Id);

        if (student == null) return NotFound();

        var results = await _context.AssignmentResults
            .Include(r => r.Assignment)
                .ThenInclude(a => a.Course)
            .Where(r => r.StudentProfileId == student.Id)
            .ToListAsync();

        return View(results);
    }
}