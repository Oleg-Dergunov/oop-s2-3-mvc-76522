using Microsoft.AspNetCore.Identity;
using VgcCollege.Domain.Models;

namespace VgcCollege.Web.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in new[] { "Admin", "Faculty", "Student" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await CreateUser(userManager, "admin@vgc.ie", "Admin@123", "Admin");
        await CreateUser(userManager, "faculty1@vgc.ie", "Faculty@123", "Faculty");
        await CreateUser(userManager, "faculty2@vgc.ie", "Faculty@123", "Faculty");
        await CreateUser(userManager, "student1@vgc.ie", "Student@123", "Student");
        await CreateUser(userManager, "student2@vgc.ie", "Student@123", "Student");
        await CreateUser(userManager, "student3@vgc.ie", "Student@123", "Student");

        if (context.Branches.Any()) return;

        var dublin = new Branch { Name = "Dublin Campus", Address = "1 O'Connell Street, Dublin 1" };
        var cork = new Branch { Name = "Cork Campus", Address = "12 Patrick Street, Cork" };
        var galway = new Branch { Name = "Galway Campus", Address = "5 Shop Street, Galway" };
        context.Branches.AddRange(dublin, cork, galway);
        await context.SaveChangesAsync();

        var f1User = await userManager.FindByEmailAsync("faculty1@vgc.ie");
        var f2User = await userManager.FindByEmailAsync("faculty2@vgc.ie");

        var faculty1 = new FacultyProfile
        {
            IdentityUserId = f1User!.Id,
            Name = "Faculty1 User",
            Email = "faculty1@vgc.ie",
            Phone = "087-1111111"
        };
        var faculty2 = new FacultyProfile
        {
            IdentityUserId = f2User!.Id,
            Name = "Faculty2 User",
            Email = "faculty2@vgc.ie",
            Phone = "087-2222222"
        };
        context.FacultyProfiles.AddRange(faculty1, faculty2);
        await context.SaveChangesAsync();

        var courseStart = new DateOnly(2026, 1, 12);
        var courseEnd = new DateOnly(2026, 5, 31);

        var cSoftware = new Course
        {
            Name = "Software Development",
            BranchId = dublin.Id,
            FacultyProfileId = faculty1.Id,
            StartDate = courseStart,
            EndDate = courseEnd
        };
        var cData = new Course
        {
            Name = "Data Science",
            BranchId = cork.Id,
            FacultyProfileId = faculty1.Id,
            StartDate = courseStart,
            EndDate = courseEnd
        };
        var cCyber = new Course
        {
            Name = "Cybersecurity",
            BranchId = galway.Id,
            FacultyProfileId = faculty2.Id,
            StartDate = courseStart,
            EndDate = courseEnd
        };
        context.Courses.AddRange(cSoftware, cData, cCyber);
        await context.SaveChangesAsync();

        var s1User = await userManager.FindByEmailAsync("student1@vgc.ie");
        var s2User = await userManager.FindByEmailAsync("student2@vgc.ie");
        var s3User = await userManager.FindByEmailAsync("student3@vgc.ie");

        var student1 = new StudentProfile
        {
            IdentityUserId = s1User!.Id,
            Name = "Student1 User",
            Email = "student1@vgc.ie",
            Phone = "086-1111111",
            Address = "10 Main Street, Dublin 2",
            DateOfBirth = new DateOnly(2000, 3, 15),
            StudentNumber = "STU001"
        };
        var student2 = new StudentProfile
        {
            IdentityUserId = s2User!.Id,
            Name = "Student2 User",
            Email = "student2@vgc.ie",
            Phone = "086-2222222",
            Address = "20 High Street, Cork",
            DateOfBirth = new DateOnly(1999, 7, 22),
            StudentNumber = "STU002"
        };
        var student3 = new StudentProfile
        {
            IdentityUserId = s3User!.Id,
            Name = "Student3 User",
            Email = "student3@vgc.ie",
            Phone = "086-3333333",
            Address = "5 Shop Street, Galway",
            DateOfBirth = new DateOnly(2001, 11, 8),
            StudentNumber = "STU003"
        };
        context.StudentProfiles.AddRange(student1, student2, student3);
        await context.SaveChangesAsync();

        var enrol1 = new CourseEnrolment
        {
            StudentProfileId = student1.Id,
            CourseId = cSoftware.Id,
            EnrolDate = new DateOnly(2026, 1, 8),
            Status = EnrolmentStatus.Active
        };
        var enrol2 = new CourseEnrolment
        {
            StudentProfileId = student2.Id,
            CourseId = cSoftware.Id,
            EnrolDate = new DateOnly(2026, 1, 8),
            Status = EnrolmentStatus.Active
        };
        var enrol3 = new CourseEnrolment
        {
            StudentProfileId = student2.Id,
            CourseId = cData.Id,
            EnrolDate = new DateOnly(2026, 1, 9),
            Status = EnrolmentStatus.Active
        };
        var enrol4 = new CourseEnrolment
        {
            StudentProfileId = student3.Id,
            CourseId = cCyber.Id,
            EnrolDate = new DateOnly(2026, 1, 9),
            Status = EnrolmentStatus.Active
        };
        context.CourseEnrolments.AddRange(enrol1, enrol2, enrol3, enrol4);
        await context.SaveChangesAsync();

        var attendanceData = new List<(CourseEnrolment enrol, int week, DateOnly date, bool present)>
        {
            (enrol1, 1, new DateOnly(2026, 1, 12), true),
            (enrol1, 2, new DateOnly(2026, 1, 19), true),
            (enrol1, 3, new DateOnly(2026, 1, 26), true),
            (enrol1, 4, new DateOnly(2026, 2, 2),  false),

            (enrol2, 1, new DateOnly(2026, 1, 12), true),
            (enrol2, 2, new DateOnly(2026, 1, 19), false),
            (enrol2, 3, new DateOnly(2026, 1, 26), true),
            (enrol2, 4, new DateOnly(2026, 2, 2),  true),

            (enrol4, 1, new DateOnly(2026, 1, 12), true),
            (enrol4, 2, new DateOnly(2026, 1, 19), true),
            (enrol4, 3, new DateOnly(2026, 1, 26), false),
            (enrol4, 4, new DateOnly(2026, 2, 2),  true),
        };
        foreach (var (enrol, week, date, present) in attendanceData)
        {
            context.AttendanceRecords.Add(new AttendanceRecord
            {
                CourseEnrolmentId = enrol.Id,
                WeekNumber = week,
                Date = date,
                Present = present
            });
        }
        await context.SaveChangesAsync();

        var assign1 = new Assignment
        {
            Title = "OOP Fundamentals Report",
            MaxScore = 100,
            DueDate = new DateOnly(2026, 2, 20),
            CourseId = cSoftware.Id
        };
        var assign2 = new Assignment
        {
            Title = "Database Design Project",
            MaxScore = 100,
            DueDate = new DateOnly(2026, 3, 20),
            CourseId = cSoftware.Id
        };
        var assign3 = new Assignment
        {
            Title = "Network Security Analysis",
            MaxScore = 100,
            DueDate = new DateOnly(2026, 2, 27),
            CourseId = cCyber.Id
        };
        context.Assignments.AddRange(assign1, assign2, assign3);
        await context.SaveChangesAsync();

        context.AssignmentResults.AddRange(
            new AssignmentResult
            {
                AssignmentId = assign1.Id,
                StudentProfileId = student1.Id,
                Score = 85,
                Feedback = "Excellent understanding of OOP principles."
            },
            new AssignmentResult
            {
                AssignmentId = assign1.Id,
                StudentProfileId = student2.Id,
                Score = 71,
                Feedback = "Good work, needs more detail on inheritance."
            },
            new AssignmentResult
            {
                AssignmentId = assign3.Id,
                StudentProfileId = student3.Id,
                Score = 88,
                Feedback = "Very thorough security analysis."
            }
        );
        await context.SaveChangesAsync();

        var exam1 = new Exam
        {
            Title = "Software Development — Semester 1 Exam",
            Date = new DateOnly(2026, 3, 10),
            MaxScore = 100,
            ResultsReleased = true,
            CourseId = cSoftware.Id
        };
        var exam2 = new Exam
        {
            Title = "Cybersecurity — Semester 1 Exam",
            Date = new DateOnly(2026, 3, 15),
            MaxScore = 100,
            ResultsReleased = false,
            CourseId = cCyber.Id
        };
        context.Exams.AddRange(exam1, exam2);
        await context.SaveChangesAsync();

        context.ExamResults.AddRange(
            new ExamResult
            {
                ExamId = exam1.Id,
                StudentProfileId = student1.Id,
                Score = 78,
                Grade = "B"
            },
            new ExamResult
            {
                ExamId = exam1.Id,
                StudentProfileId = student2.Id,
                Score = 64,
                Grade = "C"
            },
            new ExamResult
            {
                ExamId = exam2.Id,
                StudentProfileId = student3.Id,
                Score = 81,
                Grade = "B+"
            }
        );
        await context.SaveChangesAsync();
    }

    private static async Task CreateUser(
        UserManager<IdentityUser> userManager,
        string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null) return;

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, role);
    }
}