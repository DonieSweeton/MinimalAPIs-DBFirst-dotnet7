namespace MinimalDBFirst.Models.DTO
{
    public class CreateUserDTO
    {
        public string UserName { get; set; } = null!;

        public string? UserEmail { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
    }
}
