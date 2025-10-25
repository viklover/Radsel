namespace Radsel.Core.Model.Battery;
/// <summary>
///     Состояние батареи
/// </summary>
public enum RadselBatteryState {
    /// <summary>
    ///     Разряд до 2 уровня
    /// </summary>
    Low2,
    /// <summary>
    ///     Разряд до 1 уровня
    /// </summary>
    Low1,
    /// <summary>
    ///     Норма
    /// </summary>
    OK,
    /// <summary>
    ///     Не использовалась
    /// </summary>
    NotUsed,
    /// <summary>
    ///     Отключена
    /// </summary>
    Disconnected
}
