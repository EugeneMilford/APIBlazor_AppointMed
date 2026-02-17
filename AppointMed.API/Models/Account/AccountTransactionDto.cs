namespace AppointMed.API.Models.Account
{
    public class AccountTransactionDto
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? AppointmentId { get; set; }
        public int? PrescriptionId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}