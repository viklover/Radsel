using Radsel.Core.Model.Event;
using Radsel.Core.Model.Gate;

namespace Radsel.Core.Test;
/// <summary>
///     Abstract test
/// </summary>
public abstract class RadselTest {
    protected static int GenerateInt() => Random.Shared.Next();
    protected static int GenerateInt(int min, int max) => Random.Shared.Next(min, max);
    protected static bool GenerateBool() => GenerateInt(1, 3) == 1;
    protected static string GenerateString() => Guid.NewGuid().ToString();
    protected static RadselEventType GenerateEventType() => (RadselEventType)GenerateInt(1, 23);
    protected static RadselEventId GenerateEventId() => new(GenerateInt());
    protected static RadselEvent GenerateEvent() => new(GenerateEventId(), GenerateEventType());
    protected static RadselGateUserConfig GenerateUserConfig() {
        var numberOfActions = GenerateInt(1, 14);
        var array = new RadselGateOutputAction?[numberOfActions];
        for (var i = 0; i < numberOfActions; ++i) {
            array[i] = GenerateOutputAction();
        }
        return new(GenerateString(), GenerateString(), array);
    }
    protected static RadselGateOutputAction? GenerateOutputAction() {
        if (GenerateBool()) {
            return null;
        }
        if (GenerateBool()) {
            if (GenerateBool()) {
                return new RadselGateOutputActionPower(true);
            }
            return new RadselGateOutputActionPower(false);
        }
        return new RadselGateOutputActionScenario($"S{GenerateInt(1, 14)}");
    }
}
