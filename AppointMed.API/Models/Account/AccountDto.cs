namespace AppointMed.API.Models.Account
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
