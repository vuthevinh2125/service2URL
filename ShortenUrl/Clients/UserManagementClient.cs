using System.Net.Http;
using System.Net.Http.Json;
using ShortenUrl.Models;

namespace ShortenUrl.Clients
{
    public class UserManagementClient : IUserManagementClient
    {
        private readonly HttpClient _httpClient;

        public UserManagementClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserModel?> GetUserByIdAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling UserManagement Service: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);

            return user != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}