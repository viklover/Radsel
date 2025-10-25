namespace Radsel.Core.Model.Event;
/// <summary>
///      Отключение внешнего питания
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventPowerFault(RadselEventId Id) : RadselEvent(Id, RadselEventType.PowerFault);
