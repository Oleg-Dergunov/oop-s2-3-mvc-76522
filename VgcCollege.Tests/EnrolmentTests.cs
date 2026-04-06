using Microsoft.EntityFrameworkCore;
using VgcCollege.Domain.Models;
using VgcCollege.Web.Data;

namespace VgcCollege.Tests;

public class EnrolmentTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task DuplicateEnrolment_IsDetectedCorrectly()
    {
        using var context = CreateContext();

        var branch = new Branch { Name = "B", Address = "A" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var course = new Course
        {
            Name = "Course",
            BranchId = branch.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 5, 31)
        };
        context.Courses.Add(course);

        var student = new StudentProfile
        {
            IdentityUserId = "u1",
            Name = "Student",
            Email = "s@s.com",
            Phone = "123",
            Address = "Addr",
            DateOfBirth = new DateOnly(2000, 1, 1),
            StudentNumber = "STU001"
        };
        context.StudentProfiles.Add(student);
        await context.SaveChangesAsync();

        context.CourseEnrolments.Add(new CourseEnrolment
        {
            StudentProfileId = student.Id,
            CourseId = course.Id,
            EnrolDate = new DateOnly(2026, 1, 1),
            Status = EnrolmentStatus.Active
        });
        await context.SaveChangesAsync();

        var isDuplicate = await context.CourseEnrolments
            .AnyAsync(e => e.StudentProfileId == student.Id
                        && e.CourseId == course.Id);

        Assert.True(isDuplicate);
    }

    [Fact]
    public async Task Faculty_OnlySeesStudentsOnTheirCourses()
    {
        using var context = CreateContext();

        var branch = new Branch { Name = "B", Address = "A" };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var faculty1 = new FacultyProfile
        {
            IdentityUserId = "f1",
            Name = "Faculty1",
            Email = "f1@f.com",
            Phone = "111"
        };
        var faculty2 = new FacultyProfile
        {
            IdentityUserId = "f2",
            Name = "Faculty2",
            Email = "f2@f.com",
            Phone = "222"
        };
        context.FacultyProfiles.AddRange(faculty1, faculty2);
        await context.SaveChangesAsync();

        var course1 = new Course
        {
            Name = "Course1",
            BranchId = branch.Id,
            FacultyProfileId = faculty1.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 5, 31)
        };
        var course2 = new Course
        {
            Name = "Course2",
            BranchId = branch.Id,
            FacultyProfileId = faculty2.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 5, 31)
        };
        context.Courses.AddRange(course1, course2);

        var student1 = new StudentProfile
        {
            IdentityUserId = "s1",
            Name = "Student1",
            Email = "s1@s.com",
            Phone = "111",
            Address = "Addr",
            DateOfBirth = new DateOnly(2000, 1, 1),
            StudentNumber = "STU001"
        };
        var student2 = new StudentProfile
        {
            IdentityUserId = "s2",
            Name = "Student2",
            Email = "s2@s.com",
            Phone = "222",
            Address = "Addr",
            DateOfBirth = new DateOnly(2000, 1, 1),
            StudentNumber = "STU002"
        };
        context.StudentProfiles.AddRange(student1, student2);
        await context.SaveChangesAsync();

        context.CourseEnrolments.AddRange(
            new CourseEnrolment
            {
                StudentProfileId = student1.Id,
                CourseId = course1.Id,
                EnrolDate = new DateOnly(2026, 1, 1),
                Status = EnrolmentStatus.Active
            },
            new CourseEnrolment
            {
                StudentProfileId = student2.Id,
                CourseId = course2.Id,
                EnrolDate = new DateOnly(2026, 1, 1),
                Status = EnrolmentStatus.Active
            }
        );
        await context.SaveChangesAsync();

        var faculty1Students = await context.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Where(e => e.Course.FacultyProfileId == faculty1.Id)
            .Select(e => e.StudentProfile)
            .ToListAsync();

        Assert.Single(faculty1Students);
        Assert.Equal("Student1", faculty1Students[0].Name);
    }
}