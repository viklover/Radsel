namespace Radsel.Core.Model.Event;
/// <summary>
///     Планировщик задач
/// </summary>
public record RadselEventSourceScheduler() : RadselEventSource(RadselEventSourceType.Scheduler);
