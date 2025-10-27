namespace Radsel.Core.Model.Gate;
/// <summary>
///     Установить состояние выхода
/// </summary>
public record RadselGateOutputActionPower : RadselGateOutputAction {
    /// <summary>
    ///     Конструктор действия
    /// </summary>
    /// <param name="state">Состояние выхода</param>
    public RadselGateOutputActionPower(bool state) : base(state ? RadselGateOutputActionType.On : RadselGateOutputActionType.Off) {}
}
