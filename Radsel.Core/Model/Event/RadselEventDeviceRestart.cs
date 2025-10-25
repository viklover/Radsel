namespace Radsel.Core.Model.Event;
/// <summary>
///     Контроллер перезапущен
/// </summary>
/// <param name="Id">Идентификатор</param>
public record RadselEventDeviceRestart(RadselEventId Id) : RadselEvent(Id, RadselEventType.DeviceRestart);
