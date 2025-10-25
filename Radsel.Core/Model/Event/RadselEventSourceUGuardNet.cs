namespace Radsel.Core.Model.Event;
/// <summary>
///     uGuard по сети
/// </summary>
/// <param name="UserName">Имя пользователя</param>
public record RadselEventSourceUGuardNet(string UserName) : RadselEventSource(RadselEventSourceType.UGuardNet);
