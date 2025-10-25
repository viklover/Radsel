namespace Radsel.Core.Model.Event;
/// <summary>
///     Вход активен
/// </summary>
/// <param name="Id">Идентификатор события/param>
/// <param name="Pin">Номер входа</param>
/// <param name="Partitions">Номера привязанных разделов</param>
public record RadselEventInputActive(RadselEventId Id, int Pin, int[]? Partitions) : RadselEvent(Id, RadselEventType.InputActive);
