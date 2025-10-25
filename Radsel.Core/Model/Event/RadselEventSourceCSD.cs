namespace Radsel.Core.Model.Event;
/// <summary>
///     CSD соединение
/// </summary>
/// <param name="Phone">Номер телефона</param>
public record RadselEventSourceCSD(string? Phone) : RadselEventSource(RadselEventSourceType.CSD);
