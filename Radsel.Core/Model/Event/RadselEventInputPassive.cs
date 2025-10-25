namespace Radsel.Core.Model.Event;
/// <summary>
///     Вход пассивен
/// </summary>
/// <param name="Id">Идентификатор события/param>
/// <param name="Pin">Номер входа</param>
/// <param name="Partitions">Номера привязанных разделов</param>
public record RadselEventInputPassive(RadselEventId Id, int Pin, int[]? Partitions) : RadselEvent(Id, RadselEventType.InputPassive);
