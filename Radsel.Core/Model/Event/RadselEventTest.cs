namespace Radsel.Core.Model.Event;
/// <summary>
///     Тестовое сообщение
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventTest(RadselEventId Id) : RadselEvent(Id, RadselEventType.Test);
