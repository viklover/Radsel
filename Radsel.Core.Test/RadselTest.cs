using Radsel.Core.Model.Event;

namespace Radsel.Core.Test;
/// <summary>
///     Abstract test
/// </summary>
public abstract class RadselTest {
    protected static int GenerateInt() => Random.Shared.Next();
    protected static int GenerateInt(int min, int max) => Random.Shared.Next(min, max);
    protected static RadselEventType GenerateEventType() => (RadselEventType)GenerateInt(1, 23);
    protected static RadselEventId GenerateEventId() => new(GenerateInt());
    protected static RadselEvent GenerateEvent() => new(GenerateEventId(), GenerateEventType());
}
