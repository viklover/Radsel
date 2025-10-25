namespace Radsel.Core.Model.Event;
/// <summary>
///     Источник изменения режима охраны
/// </summary>
/// <param name="Type">Тип источника изменения режима охраны</param>
public abstract record RadselEventSource(RadselEventSourceType Type);
