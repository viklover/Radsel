namespace Radsel.Core.Model.Event;
/// <summary>
///     Восстановление внешнего питания
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventPowerRecovery(RadselEventId Id) : RadselEvent(Id, RadselEventType.PowerRecovery);
