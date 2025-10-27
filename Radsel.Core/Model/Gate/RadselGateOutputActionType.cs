namespace Radsel.Core.Model.Gate;
/// <summary>
///     Тип воздействия на выход
/// </summary>
public enum RadselGateOutputActionType {
    /// <summary>
    ///     Включить
    /// </summary>
    On,
    /// <summary>
    ///     Выключить
    /// </summary>
    Off,
    /// <summary>
    ///     Выполнить сценарий
    /// </summary>
    Scenario
}
