namespace Radsel.Core.Model.Event;
/// <summary>
///     Вскрытие корпуса контроллера
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventCaseOpen(RadselEventId Id) : RadselEvent(Id, RadselEventType.CaseOpen);
