namespace Models.DTOs
{
    public class UserUpdateDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? UserIdCard { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Password { get; set; }
    }
}