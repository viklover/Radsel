using System.Text;

namespace Radsel.Core.Model;
/// <summary>
///     Radsel credentials
/// </summary>
/// <param name="Username">Username</param>
/// <param name="Password">Password</param>
/// <param name="IMEI">IMEI of device</param>
public record RadselCredentials(string Username, string Password, string IMEI) {
    /// <summary>
    ///     Authentication token
    /// </summary>
    public string Token { 
        get {
            var auth = $"{Username}@{IMEI}:{Password}";
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
            return token;
        }
    }
}
