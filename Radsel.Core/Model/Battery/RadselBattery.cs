namespace Radsel.Core.Model.Battery;
/// <summary>
///     Заряд батареи
/// </summary>
/// <param name="State">Состояние</param>
/// <param name="Charge">Заряд в процентах</param>
public record RadselBattery(RadselBatteryState State, int? Charge);
