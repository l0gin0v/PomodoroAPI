using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class DurationTests
{
    [Theory]
    [InlineData(-1, 5, 20)]
    [InlineData(1, -5, 20)]
    [InlineData(1, 5, -20)]
    [InlineData(0, 5, 20)]
    [InlineData(1, 0, 20)]
    [InlineData(1, 5, 0)]
    public void FromMinutes_WithNegativeOrZeroValues_ThrowArgumentOutOfRangeException(
        double workMinutes, double shortBreakMinutes, double longBreakMinutes)
    {
        Action createAction = ()
            => Duration.FromMinutes(workMinutes, shortBreakMinutes, longBreakMinutes);
        
        createAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Default_ShouldCreateCorrectDuration()
    {
        var duration = Duration.Classic();
        
        duration.WorkTime.Should().Be(TimeSpan.FromMinutes(25));
        duration.ShortBreakTime.Should().Be(TimeSpan.FromMinutes(5));
        duration.LongBreakTime.Should().Be(TimeSpan.FromMinutes(20));
    }

    [Fact]
    public void LongFocus_ShouldCreateCorrectDuration()
    {
        var duration = Duration.LongFocus();
        
        duration.WorkTime.Should().Be(TimeSpan.FromMinutes(50));
        duration.ShortBreakTime.Should().Be(TimeSpan.FromMinutes(10));
        duration.LongBreakTime.Should().Be(TimeSpan.FromMinutes(40));
    }
    
    [Theory]
    [InlineData(PomodoroStage.Work, 25)]
    [InlineData(PomodoroStage.ShortBreak, 5)]
    [InlineData(PomodoroStage.LongBreak, 20)]
    public void GetTotalTimeForStage_ShouldReturnCorrespondingDuration(
        PomodoroStage stage,
        double expectedMinutes)
    {
        var duration = Duration.Classic();
        
        var actualMinutes = duration.GetTotalTimeForStage(stage);
        
        actualMinutes.Should().Be(TimeSpan.FromMinutes(expectedMinutes));
    }

    [Fact]
    public void GetTotalTimeForStage_WithUndefinedStage_ShouldThrowArgumentOutOfRangeException()
    {
        var duration = Duration.Classic();
        var invalidStage = (PomodoroStage)(-1);
        
        Action action = () => duration.GetTotalTimeForStage(invalidStage);
        
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}