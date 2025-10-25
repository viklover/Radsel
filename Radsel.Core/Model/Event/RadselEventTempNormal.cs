namespace Radsel.Core.Model.Event;
/// <summary>
///     Температура платы вернулась в допустимый диапазон
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventTempNormal(RadselEventId Id) : RadselEvent(Id, RadselEventType.TempNormal);
