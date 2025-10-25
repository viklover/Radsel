namespace Radsel.Core.Model.Event;
/// <summary>
///     SMS команда
/// </summary>
/// <param name="Phone">Номер телефона</param>
public record RadselEventSourceSMS(string? Phone) : RadselEventSource(RadselEventSourceType.SMS);
