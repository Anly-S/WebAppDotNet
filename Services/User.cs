using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Interfaces
{
    public interface IUserService
    {
        ApiResponse CreateUser(string username, string password, string role);
        ApiResponse AuthenticateUser(string username, string password);
        ApiResponse IsUserInRole(string username, string role);
    }
}
