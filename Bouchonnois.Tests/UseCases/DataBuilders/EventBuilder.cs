using Bouchonnois.Domain;

namespace Bouchonnois.Tests.UseCases.DataBuilders;

public class EventBuilder
{
    private readonly DateTime _happenedAt = new(2024, 4, 25, 0, 0, 0);
    private readonly string _message;
    private int _hours;
    private int _minutes;
    private int _seconds;

    public EventBuilder(string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message);
        _message = message;
    }

    public EventBuilder AtHour(int hours)
    {
        _hours = hours;
        return this;
    }

    public EventBuilder AndMinutes(int minutes)
    {
        _minutes = minutes;
        return this;
    }

    public EventBuilder AndSeconds(int seconds)
    {
        _seconds = seconds;
        return this;
    }

    public Event Build() => new(
        _happenedAt
            .AddHours(_hours)
            .AddMinutes(_minutes)
            .AddSeconds(_seconds),
        _message);
}
