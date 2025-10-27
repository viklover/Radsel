namespace Radsel.Core.Model.Gate;
/// <summary>
///     Выполнить сценарий
/// </summary>
/// <param name="Name">Наименование сцнеария (к примеру - S14)</param>
public record RadselGateOutputActionScenario(string Name) : RadselGateOutputAction(RadselGateOutputActionType.Scenario);
