namespace AppointMed.API.Data
{
    public class AccountTransaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; } // "Appointment", "Prescription", "Payment"
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? AppointmentId { get; set; }
        public int? PrescriptionId { get; set; }
        public DateTime TransactionDate { get; set; }

        // Navigation properties
        public Account Account { get; set; }
        public Appointment Appointment { get; set; }
        public Prescription Prescription { get; set; }
    }
}
