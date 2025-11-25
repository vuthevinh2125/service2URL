namespace ShortenUrl.Models
{
    // Model để nhận dữ liệu User từ Service 3
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}