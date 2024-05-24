namespace FinanceFolio.Models
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public string? Category { get; set; }

        public decimal? Amount { get; set; }

        public DateTime ExpenseDate{ get; set;}
    }
}
