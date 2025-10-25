namespace Radsel.Core.Model.Event;
/// <summary>
///     Переведен в режим НАБЛЮДЕНИЕ
/// </summary>
/// <param name="Id">Идентификатор события</param>
/// <param name="Source">Источник изменения режима наблюдения</param>
/// <param name="Partition">Номер раздела</param>
public record RadselEventDisarm(RadselEventId Id, RadselEventSource Source, int? Partition) : RadselEvent(Id, RadselEventType.Disarm);
