using Domain.Enums;

namespace Domain.ValueObjects;

public sealed class Duration
{
    public TimeSpan WorkTime { get; }
    public TimeSpan ShortBreakTime { get; }
    public TimeSpan LongBreakTime { get; }

    private Duration(TimeSpan workTime, TimeSpan shortBreakTime, TimeSpan longBreakTime)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(workTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shortBreakTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(longBreakTime, TimeSpan.Zero);
        
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

    public static Duration Default() => FromMinutes(25, 5, 20);
    public static Duration LongFocus() => FromMinutes(50, 10, 40);

    public TimeSpan GetTotalTimeForStage(PomodoroStage stage) => stage switch
    {
        PomodoroStage.Work => WorkTime,
        PomodoroStage.ShortBreak => ShortBreakTime,
        PomodoroStage.LongBreak => LongBreakTime,
        _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
    };
}