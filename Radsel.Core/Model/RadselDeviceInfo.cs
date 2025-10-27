namespace Radsel.Core.Model;
/// <summary>
///     Device info
/// </summary>
/// <param name="DeviceType">Тип контроллера</param>
/// <param name="DeviceMod">Модификация</param>
/// <param name="ExtBoard">Плата расширения</param>
/// <param name="InputsCount">Количество входов</param>
/// <param name="PartitionsCount">Количество разделов</param>
/// <param name="HwVer">Аппаратная версия</param>
/// <param name="FwVer">Версия прошивки</param>
/// <param name="BootVer">Версия загрузчика</param>
/// <param name="FwBuildDate">Дата сборки прошивки</param>
/// <param name="CountryCode">Код страны</param>
/// <param name="Serial">Серийный номер</param>
/// <param name="IMEI">IMEI</param>
/// <param name="uGuardVerCode">Код минимальной версии uGuard</param>
public record RadselDeviceInfo(
    string DeviceType,
    string DeviceMod,
    string ExtBoard,
    int InputsCount,
    int PartitionsCount,
    string HwVer,
    string FwVer,
    string BootVer,
    string FwBuildDate,
    string CountryCode,
    string Serial,
    string IMEI,
    int uGuardVerCode
);
