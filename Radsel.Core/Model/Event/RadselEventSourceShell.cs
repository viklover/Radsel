namespace Radsel.Core.Model.Event;
/// <summary>
///     CCU shell
/// </summary>
/// <param name="UserName">Имя пользователя</param>
public record RadselEventSourceShell(string UserName) : RadselEventSource(RadselEventSourceType.Shell);
