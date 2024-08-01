using WebApplicationDotNET.Interfaces;
using WebApplicationDotNET.Models;

namespace WebApplicationDotNET.Implementations
{
    public class UserService : IUserService
    {
        private readonly string usersFilePath = "C:\\Users\\anly.s\\source\\repos\\WebApplicationDotNET\\WebApplicationDotNET\\DataFiles\\users.csv";
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        private IEnumerable<UserDetails> ReadUsersFromCsv(string filePath)
        {
            var users = new List<UserDetails>();

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var values = line.Split(',');

                    if (values.Length != 3)
                    {
                        continue;
                    }

                    var user = new UserDetails
                    {
                        Username = values[0],
                        Password = values[1],
                        Role = values[2]
                    };

                    users.Add(user);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading the users CSV file at {FilePath}.", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the users CSV file.");
            }

            return users;
        }

        private void WriteUsersToCsv(IEnumerable<UserDetails> users)
        {
            try
            {
                var lines = users.Select(u => $"{u.Username},{u.Password},{u.Role}");
                File.WriteAllLines(usersFilePath, lines);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error writing to the users CSV file at {FilePath}.", usersFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while writing to the users CSV file.");
            }
        }

        public ApiResponse CreateUser(string username, string password, string role)
        {
            var response = new ApiResponse();
            try
            {
                var users = ReadUsersFromCsv(usersFilePath).ToList();

                if (users.Any(u => u.Username == username))
                {
                    _logger.LogWarning("User creation failed: User with username {Username} already exists.", username);
                    response.status = "fail";
                    response.error = "User already exists.";
                    return response;
                }

                var newUser = new UserDetails
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                users.Add(newUser);
                WriteUsersToCsv(users);

                _logger.LogInformation("User created successfully with username: {Username}", username);
                response.status = "success";
                response.data = newUser;
                response.count = 1;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user with username: {Username}", username);
                response.status = "fail";
                response.error = ex.Message;
                return response;
            }
        }

        public ApiResponse AuthenticateUser(string username, string password)
        {
            var response = new ApiResponse();
            try
            {
                var users = ReadUsersFromCsv(usersFilePath);
                var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    _logger.LogInformation("User authenticated successfully with username: {Username}", username);
                    response.status = "success";
                    response.data = user;
                    response.count = 1;
                }
                else
                {
                    _logger.LogWarning("Authentication failed for username: {Username}", username);
                    response.status = "fail";
                    response.error = "Invalid username or password.";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while authenticating user with username: {Username}", username);
                response.status = "fail";
                response.error = ex.Message;
                return response;
            }
        }

        public ApiResponse IsUserInRole(string username, string role)
        {
            var response = new ApiResponse();
            try
            {
                var user = ReadUsersFromCsv(usersFilePath).FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    var isInRole = user.Role.Equals(role, StringComparison.OrdinalIgnoreCase);
                    if (isInRole)
                    {
                        _logger.LogInformation("User {Username} is in role {Role}.", username, role);
                        response.status = "success";
                        response.data = new { Username = username, Role = role };
                        response.count = 1;
                    }
                    else
                    {
                        _logger.LogWarning("User {Username} is not in role {Role}.", username, role);
                        response.status = "fail";
                        response.error = "User is not in the specified role.";
                    }
                }
                else
                {
                    _logger.LogWarning("User with username {Username} not found.", username);
                    response.status = "fail";
                    response.error = "User not found.";
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking role for user with username: {Username}", username);
                response.status = "fail";
                response.error = ex.Message;
                return response;
            }
        }
    }
}
