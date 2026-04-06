using VgcCollege.Domain.Helpers;

namespace VgcCollege.Tests;

public class GradeCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsA_WhenScoreIs70OrAbove()
    {
        Assert.Equal("A", GradeCalculator.Calculate(70, 100));
        Assert.Equal("A", GradeCalculator.Calculate(100, 100));
    }

    [Fact]
    public void Calculate_ReturnsB_WhenScoreIs60To69()
    {
        Assert.Equal("B", GradeCalculator.Calculate(60, 100));
        Assert.Equal("B", GradeCalculator.Calculate(69, 100));
    }

    [Fact]
    public void Calculate_ReturnsC_WhenScoreIs50To59()
    {
        Assert.Equal("C", GradeCalculator.Calculate(50, 100));
        Assert.Equal("C", GradeCalculator.Calculate(59, 100));
    }

    [Fact]
    public void Calculate_ReturnsD_WhenScoreIs40To49()
    {
        Assert.Equal("D", GradeCalculator.Calculate(40, 100));
        Assert.Equal("D", GradeCalculator.Calculate(49, 100));
    }

    [Fact]
    public void Calculate_ReturnsE_WhenScoreBelowA40()
    {
        Assert.Equal("E", GradeCalculator.Calculate(39, 100));
        Assert.Equal("E", GradeCalculator.Calculate(0, 100));
    }

    [Fact]
    public void Calculate_ReturnsNA_WhenMaxScoreIsZero()
    {
        Assert.Equal("N/A", GradeCalculator.Calculate(0, 0));
    }
}