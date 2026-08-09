using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects;

public sealed class Duration : ValueObject
{
    public TimeSpan WorkTime { get; }
    public TimeSpan ShortBreakTime { get; }
    public TimeSpan LongBreakTime { get; }

    private Duration(TimeSpan workTime, TimeSpan shortBreakTime, TimeSpan longBreakTime)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(workTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shortBreakTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(longBreakTime, TimeSpan.Zero);

        if (shortBreakTime >= workTime)
            throw new ArgumentException("Short break must be shorter than work time",
                nameof(shortBreakTime));
        
        if (shortBreakTime >= longBreakTime)
            throw new ArgumentException("Short break must be shorter than long break",
                nameof(shortBreakTime));

        if (longBreakTime >= workTime)
            throw new ArgumentException("Long break must be shorter than work time",
                nameof(longBreakTime));
        
        WorkTime = workTime;
        ShortBreakTime = shortBreakTime;
        LongBreakTime = longBreakTime;
    }
    
    public static Duration FromMinutes(double workMinutes,
        double shortBreakMinutes, double longBreakMinutes)
    {
        return new Duration(
            TimeSpan.FromMinutes(workMinutes),
            TimeSpan.FromMinutes(shortBreakMinutes),
            TimeSpan.FromMinutes(longBreakMinutes)
        );
    }

    public static Duration FromMinutes(double workMinutes)
    {
        var shortBreakMinutes = workMinutes / 5;
        var longBreakMinutes = shortBreakMinutes * 4;
        
        return FromMinutes(workMinutes, shortBreakMinutes, longBreakMinutes);
    }

    public static Duration Classic() => FromMinutes(25, 5, 20);
    public static Duration LongFocus() => FromMinutes(50, 10, 40);

    public TimeSpan GetTotalTimeForStage(PomodoroStage stage) => stage switch
    {
        PomodoroStage.Work => WorkTime,
        PomodoroStage.ShortBreak => ShortBreakTime,
        PomodoroStage.LongBreak => LongBreakTime,
        _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
    };

    public (double WorkMinutes, double ShortBreakMinutes, double LongBreakMinutes) ToMinutes()
        => (WorkTime.TotalMinutes, ShortBreakTime.TotalMinutes, LongBreakTime.TotalMinutes);

    public override string ToString()
        => $"Work: {WorkTime.Minutes}m, Short: {ShortBreakTime.Minutes}m, " +
           $"Long: {LongBreakTime.Minutes}m";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return WorkTime;
        yield return ShortBreakTime;
        yield return LongBreakTime;
    }

    protected override IEnumerable<IComparable> GetComparisonComponents()
    {
        yield return WorkTime;
        yield return ShortBreakTime;
        yield return LongBreakTime;
    }
}