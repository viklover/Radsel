namespace Radsel.Core.Model;
/// <summary>
///     Состояние входа
/// </summary>
/// <param name="IsActive">Состояние входа</param>
/// <param name="Voltage">Напряжение входа (значение в дискретах)</param>
public record RadselInput(bool IsActive, int Voltage);
