namespace Radsel.Core.Model.Event;
/// <summary>
///     Новые записи в журнале управления шлагбаумом
/// </summary>
/// <param name="Id">Идентификатор события</param>
public record RadselEventNewGateRecords(RadselEventId Id) : RadselEvent(Id, RadselEventType.NewGateRecords);
