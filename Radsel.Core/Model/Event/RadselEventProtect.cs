namespace Radsel.Core.Model.Event;
/// <summary>
///     Переведен в режим ЗАЩИТА
/// </summary>
/// <param name="Id">Идентификатор события</param>
/// <param name="Source">Источник изменения режима наблюдения</param>
/// <param name="Partition">Номер раздела</param>
public record RadselEventProtect(RadselEventId Id, RadselEventSource Source, int? Partition) : RadselEvent(Id, RadselEventType.Protect);
