namespace Radsel.Core.Model.Event;
/// <summary>
///     Разряд батареи до 1 уровня
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventBatteryLow1(RadselEventId Id) : RadselEvent(Id, RadselEventType.BatteryLow1);
