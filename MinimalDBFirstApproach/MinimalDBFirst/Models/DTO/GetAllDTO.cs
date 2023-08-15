namespace MinimalDBFirst.Models.DTO
{
    public class GetAllDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string? UserEmail { get; set; }
    }
}
