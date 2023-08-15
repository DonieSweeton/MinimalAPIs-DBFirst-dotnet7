namespace MinimalDBFirst.Models.DTO
{
    public class EditUserDTO
    {
        public string UserName { get; set; } = null!;

        public string? UserEmail { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
