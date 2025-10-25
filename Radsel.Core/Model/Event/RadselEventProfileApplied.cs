namespace Radsel.Core.Model.Event;
/// <summary>
///     Применен профиль
/// </summary>
/// <param name="Id">Идентификатор события</param>
/// <param name="Number">Номер профиля</param>
public record RadselEventProfileApplied(RadselEventId Id, int Number) : RadselEvent(Id, RadselEventType.ProfileApplied);
