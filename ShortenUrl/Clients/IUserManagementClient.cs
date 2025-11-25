using ShortenUrl.Models;

namespace ShortenUrl.Clients
{
    // Interface để định nghĩa các phương thức gọi sang Service 3
    // Service 2 sẽ dùng Interface này để giao tiếp với Service 3
    public interface IUserManagementClient
    {
        // Lấy thông tin người dùng dựa trên ID (Ví dụ: dùng trong Authorize filter)
        Task<UserModel?> GetUserByIdAsync(string userId);

        // Kiểm tra xem user có quyền Admin hay không
        Task<bool> IsUserAdminAsync(string userId);
    }
}