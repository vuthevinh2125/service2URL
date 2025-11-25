using System.Net.Http;
using System.Net.Http.Json;
using ShortenUrl.Models;

namespace ShortenUrl.Clients
{
    // Class triển khai logic gọi API
    public class UserManagementClient : IUserManagementClient
    {
        private readonly HttpClient _httpClient;

        // Constructor nhận HttpClient đã được cấu hình (tên "UserManagementService")
        public UserManagementClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserModel?> GetUserByIdAsync(string userId)
        {
            try
            {
                // Gọi API thực tế của Service 3. Giả định API là /api/users/{userId}
                var response = await _httpClient.GetAsync($"api/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log lỗi kết nối (nếu Service 3 bị sập hoặc URL sai)
                Console.WriteLine($"Error calling UserManagement Service: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);

            // Giả định Role được lưu trong field "Role" của UserModel
            return user != null && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}