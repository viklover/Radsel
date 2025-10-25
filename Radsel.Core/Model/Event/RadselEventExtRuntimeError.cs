namespace Radsel.Core.Model.Event;
/// <summary>
///     Ошибка выполнения программы EXT
/// </summary>
/// <param name="Id">Идентификатор события</param>
/// <param name="ErrorCode">Код ошибки</param>
public record RadselEventExtRuntimeError(RadselEventId Id, int ErrorCode) : RadselEvent(Id, RadselEventType.ExtRuntimeError);
