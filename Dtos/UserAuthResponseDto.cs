namespace XeniaRegistrationBackend.Dtos
{
    public class UserAuthResponseDto
    {
        public int UserId { get; set; }   
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
