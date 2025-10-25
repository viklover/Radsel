namespace Radsel.Core.Model.Event;
/// <summary>
///     Контроллер включен
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventDeviceOn(RadselEventId Id) : RadselEvent(Id, RadselEventType.DeviceOn);
