namespace Radsel.Core.Model.Event;
/// <summary>
///     Radsel event identifier
/// </summary>
/// <param name="id">Id</param>
public class RadselEventId(int id) {
    /// <summary>
    ///     Identifier
    /// </summary>
    public int Id { get => id; }
    /// <summary>
    ///     Compare identifier with another object
    /// </summary>
    /// <param name="obj">Another object</param>
    /// <returns>Result <see cref="bool"/></returns>
    public override bool Equals(object? obj) {
        if (obj is RadselEventId eventId) {
            return eventId.Id.Equals(id);
        }
        return false;
    }
    /// <summary>
    ///     Conert identifier to string
    /// </summary>
    /// <returns>String representation</returns>
    public override string ToString() => Id.ToString();
    /// <summary>
    ///     Get hash code
    /// </summary>
    /// <returns>Integer</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
