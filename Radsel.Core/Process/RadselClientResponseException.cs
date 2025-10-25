using Radsel.Core.Model;

namespace Radsel.Core.Process;
/// <summary>
///     Unexpected response status code
/// </summary>
/// <param name="status">Status</param>
public class RadselClientResponseException(RadselResponseStatus status) : RadselException(
    $"Unexpected response status code = {status.Code}: {status.Description}"
) {
    public RadselResponseStatus Status { get; } = status;
}
