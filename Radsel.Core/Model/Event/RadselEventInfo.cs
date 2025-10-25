namespace Radsel.Core.Model.Event;
/// <summary>
///     Информационное сообщение
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventInfo(RadselEventId Id) : RadselEvent(Id, RadselEventType.Info);
