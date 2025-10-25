namespace Radsel.Core.Model;
/// <summary>
///     Состояние охраны раздела
/// </summary>
public enum RadselPartitionState {
    /// <summary>
    ///     Охрана
    /// </summary>
    Arm,
    /// <summary>
    ///     Наблюдение
    /// </summary>
    Disarm,
    /// <summary>
    ///     Защита
    /// </summary>
    Protect
}
