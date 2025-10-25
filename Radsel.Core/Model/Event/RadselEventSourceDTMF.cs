namespace Radsel.Core.Model.Event;
/// <summary>
///     Голосовое меню
/// </summary>
/// <param name="Phone">Номер телефона</param>
public record class RadselEventSourceDTMF(string? Phone) : RadselEventSource(RadselEventSourceType.DTMF);
