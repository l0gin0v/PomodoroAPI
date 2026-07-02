using Domain.Enums;

namespace Domain.ValueObjects;

public sealed class Duration : IEquatable<Duration>, IComparable<Duration>
{
    public TimeSpan WorkTime { get; }
    public TimeSpan ShortBreakTime { get; }
    public TimeSpan LongBreakTime { get; }

    private Duration(TimeSpan workTime, TimeSpan shortBreakTime, TimeSpan longBreakTime)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(workTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shortBreakTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(longBreakTime, TimeSpan.Zero);

        // TODO: разобраться с бизнес-правилами
        if (shortBreakTime >= workTime)
            throw new ArgumentException("Short break must be shorter than work time",
                nameof(shortBreakTime));
        
        if (shortBreakTime >= longBreakTime)
            throw new ArgumentException("Short break must be shorter than long break",
                nameof(shortBreakTime));
        
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

    public static Duration Classic() => FromMinutes(25, 5, 20);
    public static Duration LongFocus() => FromMinutes(50, 10, 40);

    public TimeSpan GetTotalTimeForStage(PomodoroStage stage) => stage switch
    {
        PomodoroStage.Work => WorkTime,
        PomodoroStage.ShortBreak => ShortBreakTime,
        PomodoroStage.LongBreak => LongBreakTime,
        _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
    };
    
    public bool Equals(Duration? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return WorkTime.Equals(other.WorkTime) &&
               ShortBreakTime.Equals(other.ShortBreakTime) &&
               LongBreakTime.Equals(other.LongBreakTime);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is Duration other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(WorkTime, ShortBreakTime, LongBreakTime);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is not Duration other)
            throw new ArgumentException("Object is not a Duration", nameof(obj));
        
        return CompareTo(other);
    }

    public int CompareTo(Duration? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        
        var workTimeComparison = WorkTime.CompareTo(other.WorkTime);
        if (workTimeComparison != 0) return workTimeComparison;
        
        var shortBreakTimeComparison = ShortBreakTime.CompareTo(other.ShortBreakTime);
        if (shortBreakTimeComparison != 0) return shortBreakTimeComparison;
        
        return LongBreakTime.CompareTo(other.LongBreakTime);
    }

    public (double WorkMinutes, double ShortBreakMinutes, double LongBreakMinutes) ToMinutes()
        => (WorkTime.TotalMinutes, ShortBreakTime.TotalMinutes, LongBreakTime.TotalMinutes);

    public override string ToString()
        => $"Work: {WorkTime.Minutes}m, Short: {ShortBreakTime.Minutes}m," +
           $"Long: {LongBreakTime.Minutes}m";
}