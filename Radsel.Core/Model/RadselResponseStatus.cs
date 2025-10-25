namespace Radsel.Core.Model;
/// <summary>
///     Ответ от сервера
/// </summary>
/// <param name="Code">Статус операции</param>
/// <param name="Description">Описание кода статуса операции</param>
public record RadselResponseStatus(int Code, string Description);