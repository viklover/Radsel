namespace Radsel.Core.Model.Event;
/// <summary>
///     Переведен в режим ОХРАНА
/// </summary>
/// <param name="Id">Идентификатор события</param>
/// <param name="Source">Источник изменения режима охраны</param>
/// <param name="Partition">Номер раздела</param>
public record RadselEventArm(RadselEventId Id, RadselEventSource Source, int? Partition) : RadselEvent(Id, RadselEventType.Arm);
