namespace Radsel.Core.Model.Gate;
/// <summary>
///     Конфигурация пользователя в рамках шлагбаума
/// </summary>
/// <param name="Phone">Номер телефона</param>
/// <param name="Username">Имя пользователя</param>
/// <param name="OutputActions">Реакция выходов (null = нет реакции)</param>
public record RadselGateUserConfig(string Phone, string Username, RadselGateOutputAction?[] OutputActions);
