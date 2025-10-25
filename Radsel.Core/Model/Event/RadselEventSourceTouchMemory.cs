namespace Radsel.Core.Model.Event;
/// <summary>
///     Ключ TouchMemory
/// </summary>
/// <param name="Key">Код ключа</param>
/// <param name="KeyName">Имя ключа</param>
public record RadselEventSourceTouchMemory(string? Key, string? KeyName) : RadselEventSource(RadselEventSourceType.TouchMemory);
