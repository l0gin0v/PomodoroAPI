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

    [Theory]
    [InlineData(50, 60, 70)]
    [InlineData(50, 10, 5)]
    [InlineData(50, 5, 70)]
    public void FromMinutes_WithInvalidBreakDurations_ThrowArgumentException(
        double workMinutes, double shortBreakMinutes, double longBreakMinutes)
    {
        Action createAction = ()
            => Duration.FromMinutes(workMinutes, shortBreakMinutes, longBreakMinutes);
        
        createAction.Should().Throw<ArgumentException>();
    }

    public static IEnumerable<object[]> FromMinutesTestData()
    {
        yield return [25, Duration.FromMinutes(25, 5, 20)]; 
        yield return [50, Duration.FromMinutes(50, 10, 40)]; 
    }

    [Theory]
    [MemberData(nameof(FromMinutesTestData))]
    public void FromMinutes_WithoutBreakDurations_ShouldCreateCorrectDuration(
        double workMinutes, Duration expected)
    {
        var actual = Duration.FromMinutes(workMinutes);
        
        actual.WorkTime.Should().Be(expected.WorkTime);
        actual.ShortBreakTime.Should().Be(expected.ShortBreakTime);
        actual.LongBreakTime.Should().Be(expected.LongBreakTime);
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

    [Fact]
    public void EqualsAndGetHashCode_ShouldAdhereToDotNetContracts()
    {
        var a = Duration.FromMinutes(25, 5, 20);
        var b = Duration.FromMinutes(25, 5, 20);
        var c = Duration.FromMinutes(25, 5, 20);
        var different = Duration.FromMinutes(50, 10, 40);
        
        a.Equals(a).Should().BeTrue();
        
        a.Equals(b).Should().BeTrue();
        b.Equals(a).Should().BeTrue();
        
        b.Equals(c).Should().BeTrue();
        a.Equals(c).Should().BeTrue();
        
        a.GetHashCode().Should().Be(b.GetHashCode());
        
        a.Equals(different).Should().BeFalse();
        a.GetHashCode().Should().NotBe(different.GetHashCode());
        
        a.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithObjectParameters_ShouldHandleAllTypesSafely()
    {
        var duration = Duration.FromMinutes(25, 5, 20);
        object sameData = Duration.FromMinutes(25, 5, 20);
        object differentData = Duration.FromMinutes(50, 10, 40);
        object wrongType = "something";
        object? nullReference = null;
        
        duration.Equals(sameData).Should().BeTrue();
        duration.Equals(differentData).Should().BeFalse();
        duration.Equals(wrongType).Should().BeFalse();
        duration.Equals(nullReference).Should().BeFalse();
    }

    [Fact]
    public void CompateTo_WithObjectParameters_ShouldHangleAllTypesSafely()
    {
        var duration = Duration.FromMinutes(25, 5, 20);
        object sameData = Duration.FromMinutes(25, 5, 20);
        object differentData = Duration.FromMinutes(50, 10, 40);
        object wrongType = "something";
        object? nullReference = null;
        
        Action createAction = () => duration.CompareTo(wrongType);

        duration.CompareTo(sameData).Should().Be(0);
        duration.CompareTo(differentData).Should().BeNegative();
        
        duration.CompareTo(nullReference).Should().BePositive();
        createAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_ShouldReturnCorrectResult()
    {
        var a = Duration.FromMinutes(25, 5, 20);
        var b = Duration.FromMinutes(25, 5, 20);
        
        var diffLongBreak = Duration.FromMinutes(25, 5, 21);
        var diffShortBreak = Duration.FromMinutes(25, 6, 20);
        var diffWorkTime = Duration.FromMinutes(26, 5, 20);
        
        a.CompareTo(b).Should().Be(0);
        a.CompareTo(a).Should().Be(0);
        
        a.CompareTo(null).Should().Be(1);
        
        a.CompareTo(diffLongBreak).Should().BeNegative();
        diffLongBreak.CompareTo(a).Should().BePositive();
        
        a.CompareTo(diffShortBreak).Should().BeNegative();
        diffShortBreak.CompareTo(a).Should().BePositive();
        
        a.CompareTo(diffWorkTime).Should().BeNegative();
        diffWorkTime.CompareTo(a).Should().BePositive();
    }

    public static IEnumerable<object[]> ToMinutesTestData()
    {
        yield return [Duration.FromMinutes(25, 5, 20), 25, 5, 20];
        yield return [Duration.FromMinutes(50, 10, 40), 50, 10, 40];
    }

    [Theory]
    [MemberData(nameof(ToMinutesTestData))]
    public void ToMinutes_ShouldReturnCorrectResult(
        Duration duration,
        double workTime, double shortBreakTime, double longBreakTime)
    {
        duration.ToMinutes().WorkMinutes.Should().Be(workTime);
        duration.ToMinutes().ShortBreakMinutes.Should().Be(shortBreakTime);
        duration.ToMinutes().LongBreakMinutes.Should().Be(longBreakTime);
    }
}