namespace Radsel.Core.Model.Event;
/// <summary>
///     Температура платы упала до нижней границы
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventTempLow(RadselEventId Id) : RadselEvent(Id, RadselEventType.TempLow);
