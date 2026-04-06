using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Models;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Admin,Faculty")]
public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AttendanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> ByCourse(int id)
    {
        var enrolment = await _context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .Include(e => e.AttendanceRecords)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrolment == null) return NotFound();

        if (User.IsInRole("Faculty"))
        {
            var user = await _userManager.GetUserAsync(User);
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.IdentityUserId == user!.Id);
            if (enrolment.Course.FacultyProfileId != faculty!.Id) return Forbid();
        }

        return View(enrolment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRecord(int enrolmentId, int weekNumber, DateOnly date, AttendanceStatus status)
    {
        if (date > DateOnly.FromDateTime(DateTime.Today))
        {
            TempData["Error"] = "Cannot add attendance for a future date.";
            return RedirectToAction(nameof(ByCourse), new { id = enrolmentId });
        }

        var exists = await _context.AttendanceRecords
            .AnyAsync(a => a.CourseEnrolmentId == enrolmentId && a.WeekNumber == weekNumber);

        if (!exists)
        {
            _context.AttendanceRecords.Add(new AttendanceRecord
            {
                CourseEnrolmentId = enrolmentId,
                WeekNumber = weekNumber,
                Date = date,
                Status = status
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(ByCourse), new { id = enrolmentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AttendanceStatus status)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null) return NotFound();

        record.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ByCourse), new { id = record.CourseEnrolmentId });
    }
}