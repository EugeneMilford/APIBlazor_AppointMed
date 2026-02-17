namespace AppointMed.API.Data
{
    public class Account
    {
        public int AccountId { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ApiUser User { get; set; }
        public virtual ICollection<AccountTransaction> Transactions { get; set; }
    }
}
