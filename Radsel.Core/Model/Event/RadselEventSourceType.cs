namespace Radsel.Core.Model.Event;
/// <summary>
///     Тип источника изменения режима охраны
/// </summary>
public enum RadselEventSourceType {
    /// <summary>
    ///     Кнопка
    /// </summary>
    Button = 1,
    /// <summary>
    ///     Вход
    /// </summary>
    Input = 2,
    /// <summary>
    ///     Планировщик задач
    /// </summary>
    Scheduler = 3,
    /// <summary>
    ///     GuardTracker по сети Modbus
    /// </summary>
    Modbus = 4,
    /// <summary>
    ///     Ключ TouchMemory
    /// </summary>
    TouchMemory = 5,
    /// <summary>
    ///     Голосовое меню
    /// </summary>
    DTMF = 6,
    /// <summary>
    ///     SMS команда
    /// </summary>
    SMS = 7,
    /// <summary>
    ///     CSD соединение
    /// </summary>
    CSD = 8,
    /// <summary>
    ///     Вызов без соединения
    /// </summary>
    Call = 9,
    /// <summary>
    ///     GuardTracker по сети
    /// </summary>
    GTNet = 10,
    /// <summary>
    ///     uGuard по сети
    /// </summary>
    UGuardNet = 11,
    /// <summary>
    ///     CCU shell
    /// </summary>
    Shell = 12
}
