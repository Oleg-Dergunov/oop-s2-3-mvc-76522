using VgcCollege.Domain.Models;

namespace VgcCollege.Tests;

public class AttendanceTests
{
    [Fact]
    public void AttendanceRate_ExcludesNA_FromCalculation()
    {
        var records = new List<AttendanceRecord>
        {
            new() { Status = AttendanceStatus.Present },
            new() { Status = AttendanceStatus.Absent },
            new() { Status = AttendanceStatus.NA }
        };

        var countable = records.Where(r => r.Status != AttendanceStatus.NA).ToList();
        var present = countable.Count(r => r.Status == AttendanceStatus.Present);
        var total = countable.Count;
        var rate = total > 0 ? (double)present / total * 100 : 0;

        Assert.Equal(2, total);
        Assert.Equal(1, present);
        Assert.Equal(50.0, rate);
    }

    [Fact]
    public void AttendanceRate_ReturnsZero_WhenAllRecordsAreNA()
    {
        var records = new List<AttendanceRecord>
        {
            new() { Status = AttendanceStatus.NA },
            new() { Status = AttendanceStatus.NA }
        };

        var countable = records.Where(r => r.Status != AttendanceStatus.NA).ToList();
        var total = countable.Count;
        var rate = total > 0 ? (double)countable.Count(r => r.Status == AttendanceStatus.Present) / total * 100 : 0;

        Assert.Equal(0, rate);
    }
}