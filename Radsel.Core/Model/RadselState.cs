using Radsel.Core.Model.Battery;
using Radsel.Core.Model.Event;

namespace Radsel.Core.Model;
/// <summary>
///     Состояние контроллера и список непрочитанных событий
/// </summary>
/// <param name="Inputs">Состояние входов</param>
/// <param name="Outputs">Состояние выходов</param>
/// <param name="Partitions">Состояние разделов</param>
/// <param name="CaseState">Состояние корпуса (открыт / закрыт)</param>
/// <param name="Power">Питание в вольтах</param>
/// <param name="Temp">Температура в Цельсия</param>
/// <param name="Balance">Состояние баланса (значение в валюте)</param>
/// <param name="Battery">Состояние батареи</param>
/// <param name="Events">Непрочитанные события</param>
public record RadselState(
    RadselInput[] Inputs,
    RadselOutput[] Outputs,
    RadselPartitionState[] Partitions,
    RadselCaseState? CaseState,
    float? Power,
    int? Temp,
    float? Balance,
    RadselBattery Battery,
    RadselEvent[] Events
);