namespace Radsel.Core.Model.Event;
/// <summary>
///     Вызов без соединения
/// </summary>
/// <param name="Phone">Номер телефона</param>
public record RadselEventSourceCall(string? Phone) : RadselEventSource(RadselEventSourceType.Call);
