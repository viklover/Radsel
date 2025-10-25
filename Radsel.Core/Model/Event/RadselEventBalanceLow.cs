namespace Radsel.Core.Model.Event;
/// <summary>
///     Баланс снизился до минимальгого значения
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventBalanceLow(RadselEventId Id) : RadselEvent(Id, RadselEventType.BalanceLow);