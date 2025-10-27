namespace Radsel.Core.Model.Gate;
/// <summary>
///     Реакция на выход
/// </summary>
/// <param name="Type">Тип воздействия</param>
public abstract record RadselGateOutputAction(RadselGateOutputActionType Type);
