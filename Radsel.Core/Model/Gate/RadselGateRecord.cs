namespace Radsel.Core.Model.Gate;
/// <summary>
///     Запись журнала
/// </summary>
/// <param name="Index">Индекс записи</param>
/// <param name="DateTime">Дата и время</param>
/// <param name="Phone">Номер телефона пользователя</param>
public record RadselGateRecord(int Index, DateTime DateTime, string Phone);
