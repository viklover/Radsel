namespace Radsel.Core.Model.Event;
/// <summary>
///     Разряд батареи до 2 уровня
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventBatteryLow2(RadselEventId Id) : RadselEvent(Id, RadselEventType.BatteryLow2);
