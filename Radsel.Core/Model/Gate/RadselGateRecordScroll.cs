namespace Radsel.Core.Model.Gate;
/// <summary>
///     Скроллинг записей журнала
/// </summary>
/// <param name="Records">Записи</param>
/// <param name="Last">Последняя запись в массиве является последней в журнале. null =   конец журнала не достигнут или отсутствие записей в запрошенном диапазоне.</param>
public record RadselGateRecordScroll(RadselGateRecord[] Records, bool? Last);
