using Domain.Enums;
using Domain.UnitTests.Common;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class DurationTests : ValueObjectTests<Duration>
{
    protected override Duration CreateInstance() 
        => Duration.Classic();

    protected override Duration CreateDifferentInstance() 
        => Duration.LongFocus();

    protected override (Duration first, Duration second) CreateEqualInstances()
    {
        var first = Duration.Classic();
        var second = Duration.FromMinutes(25, 5, 20);
        return (first, second);
    }

    [Fact]
    public void FromMinutes_ValidInput_CreatesDuration()
    {
        var duration = Duration.FromMinutes(30, 10, 20);
        
        duration.WorkTime.TotalMinutes.Should().Be(30);
        duration.ShortBreakTime.TotalMinutes.Should().Be(10);
        duration.LongBreakTime.TotalMinutes.Should().Be(20);
    }

    [Fact]
    public void FromMinutes_InvalidInput_ThrowsException()
    {
        Action act = () => Duration.FromMinutes(25, 30, 20);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Short break must be shorter than work time*");

        act = () => Duration.FromMinutes(25, 5, 1);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Short break must be shorter than long break*");
        
        act = () => Duration.FromMinutes(25, 5, 30);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Long break must be shorter than work time*");
    }

    [Fact]
    public void FromMinutes_WithOnlyWorkTime_CreatesProportionalBreaks()
    {
        var duration = Duration.FromMinutes(30);
        
        duration.WorkTime.TotalMinutes.Should().Be(30);
        duration.ShortBreakTime.TotalMinutes.Should().Be(6); // 30/5
        duration.LongBreakTime.TotalMinutes.Should().Be(24); // 6*4
    }

    [Fact]
    public void GetTotalTimeForStage_ReturnsCorrectTime()
    {
        var duration = Duration.Classic();
        
        duration.GetTotalTimeForStage(PomodoroStage.Work)
            .Should().Be(TimeSpan.FromMinutes(25));
        
        duration.GetTotalTimeForStage(PomodoroStage.ShortBreak)
            .Should().Be(TimeSpan.FromMinutes(5));
        
        duration.GetTotalTimeForStage(PomodoroStage.LongBreak)
            .Should().Be(TimeSpan.FromMinutes(20));
    }

    [Fact]
    public void GetTotalTimeForStage_InvalidStage_ThrowsException()
    {
        var duration = Duration.Classic();
        
        Action act = () => duration.GetTotalTimeForStage((PomodoroStage)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToMinutes_ReturnsCorrectTuple()
    {
        var duration = Duration.Classic();
        var (work, shortBreak, longBreak) = duration.ToMinutes();
        
        work.Should().Be(25);
        shortBreak.Should().Be(5);
        longBreak.Should().Be(20);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var duration = Duration.Classic();
        
        duration.ToString().Should().Be("Work: 25m, Short: 5m, Long: 20m");
    }

    [Fact]
    public void Classic_ReturnsCorrectDuration()
    {
        var duration = Duration.Classic();
        
        duration.WorkTime.TotalMinutes.Should().Be(25);
        duration.ShortBreakTime.TotalMinutes.Should().Be(5);
        duration.LongBreakTime.TotalMinutes.Should().Be(20);
    }

    [Fact]
    public void LongFocus_ReturnsCorrectDuration()
    {
        var duration = Duration.LongFocus();
        
        duration.WorkTime.TotalMinutes.Should().Be(50);
        duration.ShortBreakTime.TotalMinutes.Should().Be(10);
        duration.LongBreakTime.TotalMinutes.Should().Be(40);
    }
}