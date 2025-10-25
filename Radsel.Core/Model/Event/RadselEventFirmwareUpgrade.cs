namespace Radsel.Core.Model.Event;
/// <summary>
///     Прошивка обновлена
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventFirmwareUpgrade(RadselEventId Id) : RadselEvent(Id, RadselEventType.FirmwareUpgrade);
