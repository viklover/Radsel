namespace Radsel.Core.Model.Event;
/// <summary>
///     Radsel event
/// </summary>
/// <param name="Id">Identifier</param>
/// <param name="Type">Type</param>
public record RadselEvent(RadselEventId Id, RadselEventType Type);
