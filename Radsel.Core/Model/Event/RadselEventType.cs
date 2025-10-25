namespace Radsel.Core.Model.Event;
/// <summary>
///     Тип события
/// </summary>
public enum RadselEventType {
    /// <summary>
    ///     Вход пассивен
    /// </summary>
    InputPassive = 1,
    /// <summary>
    ///     Вход активен
    /// </summary>
    InputActive = 2,
    /// <summary>
    ///     Восстановление внешнего питания
    /// </summary>
    PowerRecovery = 3,
    /// <summary>
    ///     Отключение внешнего питания
    /// </summary>
    PowerFault = 4,
    /// <summary>
    ///     Разряд батареи до 1 уровня
    /// </summary>
    BatteryLow1 = 5,
    /// <summary>
    ///     Разряд батареи до 2 уровня
    /// </summary>
    BatteryLow2 = 6,
    /// <summary>
    ///     Баланс снизился до минимальгого значения
    /// </summary>
    BalanceLow = 7,
    /// <summary>
    ///     Температура платы упала до нижней границы
    /// </summary>
    TempLow = 8,
    /// <summary>
    ///     Температура платы вернулась в допустимый диапазон
    /// </summary>
    TempNormal = 9,
    /// <summary>
    ///     Температура платы поднялась до верхней границы
    /// </summary>
    TempHigh = 10,
    /// <summary>
    ///     Вскрытие корпуса контроллера
    /// </summary>
    CaseOpen = 11,
    /// <summary>
    ///     Тестовое сообщение
    /// </summary>
    Test = 12,
    /// <summary>
    ///     Информационное сообщение
    /// </summary>
    Info = 13,
    /// <summary>
    ///     Переведен в режим ОХРАНА
    /// </summary>
    Arm = 14,
    /// <summary>
    ///     Переведен в режим НАБЛЮДЕНИЕ
    /// </summary>
    Disarm = 15,
    /// <summary>
    ///     Переведен в режим ЗАЩИТА
    /// </summary>
    Protect = 16,
    /// <summary>
    ///     Применен профиль
    /// </summary>
    ProfileApplied = 17,
    /// <summary>
    ///     Контроллер включен
    /// </summary>
    DeviceOn = 18,
    /// <summary>
    ///     Контроллер перезапущен
    /// </summary>
    DeviceRestart = 19,
    /// <summary>
    ///     Прошивка обновлена
    /// </summary>
    FirmwareUpgrade = 20,
    /// <summary>
    ///     Ошибка выполнения программы EXT
    /// </summary>
    ExtRuntimeError = 21,
    /// <summary>
    ///     Новые записи в журнале управления шлагбаумом
    /// </summary>
    NewGateRecords = 22
}