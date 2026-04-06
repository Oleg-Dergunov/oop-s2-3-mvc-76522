using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Models;
using VgcCollege.Web.Data;

namespace VgcCollege.Tests;

public class ExamResultVisibilityTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Student_CannotSeeUnreleasedExamResults()
    {
        using var context = CreateContext();

        var branch = new Branch { Name = "Test Branch", Address = "Test Address" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Test Course",
            BranchId = branch.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 5, 31)
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var exam = new Exam
        {
            Title = "Test Exam",
            Date = new DateOnly(2026, 3, 1),
            MaxScore = 100,
            ResultsReleased = false,
            CourseId = course.Id
        };
        context.Exams.Add(exam);

        var student = new StudentProfile
        {
            IdentityUserId = "user1",
            Name = "Test Student",
            Email = "test@test.com",
            Phone = "123",
            Address = "123 Street",
            DateOfBirth = new DateOnly(2000, 1, 1),
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        await context.SaveChangesAsync();

        context.ExamResults.Add(new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 75,
            Grade = "A"
        });
        await context.SaveChangesAsync();

        var visibleResults = await context.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.StudentProfileId == student.Id
                     && r.Exam.ResultsReleased == true)
            .ToListAsync();

        Assert.Empty(visibleResults);
    }

    [Fact]
    public async Task Student_CanSeeReleasedExamResults()
    {
        using var context = CreateContext();

        var branch = new Branch { Name = "Test Branch", Address = "Test Address" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Test Course",
            BranchId = branch.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 5, 31)
        };
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var exam = new Exam
        {
            Title = "Test Exam",
            Date = new DateOnly(2026, 3, 1),
            MaxScore = 100,
            ResultsReleased = true,
            CourseId = course.Id
        };
        context.Exams.Add(exam);

        var student = new StudentProfile
        {
            IdentityUserId = "user1",
            Name = "Test Student",
            Email = "test@test.com",
            Phone = "123",
            Address = "123 Street",
            DateOfBirth = new DateOnly(2000, 1, 1),
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        await context.SaveChangesAsync();

        context.ExamResults.Add(new ExamResult
        {
            ExamId = exam.Id,
            StudentProfileId = student.Id,
            Score = 75,
            Grade = "A"
        });
        await context.SaveChangesAsync();

        var visibleResults = await context.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.StudentProfileId == student.Id
                     && r.Exam.ResultsReleased == true)
            .ToListAsync();

        Assert.Single(visibleResults);
        Assert.Equal(75, visibleResults[0].Score);
    }
}