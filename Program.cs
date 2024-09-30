public class Transaction
{
    public int TransactionID { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } // "Income" or "Expense"
    public string Category { get; set; } // e.g., Groceries, Rent, Salary, etc.
    public string Description { get; set; }
}

public class Budget
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal IncomeGoal { get; set; }
    public decimal ExpenseLimit { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal ActualExpenses { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    public decimal CalculateSavings()
    {
        return ActualIncome - ActualExpenses;
    }
}

public class FinanceTracker
{
    private List<Budget> Budgets = new List<Budget>();

    public void SetBudget(int month, int year, decimal incomeGoal, decimal expenseLimit)
    {
        Budgets.Add(new Budget
        {
            Month = month,
            Year = year,
            IncomeGoal = incomeGoal,
            ExpenseLimit = expenseLimit
        });
    }

    public void AddTransaction(int month, int year, Transaction transaction)
    {
        var budget = Budgets.FirstOrDefault(b => b.Month == month && b.Year == year);
        if (budget != null)
        {
            budget.Transactions.Add(transaction);
            if (transaction.Type == "Income")
                budget.ActualIncome += transaction.Amount;
            else
                budget.ActualExpenses += transaction.Amount;
        }
    }

    public IEnumerable<Transaction> ListTransactionsByMonth(int month, int year)
    {
        var budget = Budgets.FirstOrDefault(b => b.Month == month && b.Year == year);
        return budget?.Transactions;
    }

    public IEnumerable<Transaction> ListTransactionsByCategory(string category)
    {
        return Budgets.SelectMany(b => b.Transactions)
                      .Where(t => t.Category == category);
    }

    public decimal CalculateSavings(int month, int year)
    {
        var budget = Budgets.FirstOrDefault(b => b.Month == month && b.Year == year);
        return budget?.CalculateSavings() ?? 0;
    }

    public IEnumerable<string> IdentifyOverspending()
    {
        return Budgets.SelectMany(b => b.Transactions)
                      .GroupBy(t => t.Category)
                      .Where(g => g.Sum(t => t.Amount) > Budgets.FirstOrDefault()?.ExpenseLimit)
                      .Select(g => $"Overspending in category: {g.Key}");
    }

    public IEnumerable<decimal> PredictFutureSpending(int categoryMonth)
    {
        return Budgets.Where(b => b.Month == categoryMonth)
                      .SelectMany(b => b.Transactions)
                      .Where(t => t.Type == "Expense")
                      .GroupBy(t => t.Category)
                      .Select(g => g.Average(t => t.Amount));
    }
}

public class Report
{
    public static string GenerateReport(FinanceTracker tracker, int month, int year)
    {
        var transactions = tracker.ListTransactionsByMonth(month, year);
        var report = "Report:\n";

        report += $"Total Income: {transactions.Where(t => t.Type == "Income").Sum(t => t.Amount)}\n";
        report += $"Total Expenses: {transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount)}\n";
        report += $"Savings: {tracker.CalculateSavings(month, year)}\n";

        return report;
    }
}

