namespace Radsel.Core;
/// <summary>
///     Radsel exception
/// </summary>
public class RadselException : Exception {
    /// <summary>
    ///     Constructor of exception
    /// </summary>
    /// <param name="message">Message</param>
    public RadselException(string message) : base(message) {}
    /// <summary>
    ///     Constructor of exception
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="innerException">Inner exception</param>
    public RadselException(string message, Exception innerException) : base(message, innerException) {}
}
