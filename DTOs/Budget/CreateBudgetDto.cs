namespace FinanceTracker.DTOs.Budget;

public class CreateBudgetDto
{
    public Guid     CategoryId  { get; set; }
    public decimal  LimitAmount { get; set; }
    public int      Month       { get; set; }
    public int      Year        { get; set; }
}