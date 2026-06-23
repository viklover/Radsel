namespace Radsel.Model.Event;
/// <summary>
///     GuardTracker по сети Modbus
/// </summary>
public record RadselEventSourceModbus() : RadselEventSource(RadselEventSourceType.Modbus);
