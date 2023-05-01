using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class AnnualSummaryTests
    {
        [Fact]
        public void TestAnnualSummary()
        {
            List<IncomeEvent> incomeEvents = new()
            {
                new IncomeEvent { DateTime = new DateTime(2020, 1, 1), Amount = 2100 },
                new IncomeEvent { DateTime = new DateTime(2020, 2, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 3, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 4, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 5, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 6, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 7, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 8, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 9, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 10, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 11, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 12, 1), Amount = 2600 },
                new IncomeEvent { DateTime = new DateTime(2020, 12, 15), Amount = 2400 },
                new IncomeEvent { DateTime = new DateTime(2020, 2, 1), Amount = 50m },
            };

            ExpenseCategory category1 = new ExpenseCategory { Name = "groceries" };
            ExpenseCategory category2 = new ExpenseCategory { Name = "car" };

            List<Expense> expenses = new()
            {
                new Expense { Category = category1, Date = new DateTime(2020, 1, 1), Amount = 10m },
                new Expense { Category = category1, Date = new DateTime(2020, 1, 2), Amount = 5m },
                new Expense { Category = category2, Date = new DateTime(2020, 1, 3), Amount = 40m }
            };

            AnnualSummary.Builder summaryBuilder = new();
            AnnualSummary.Builder summaryBuilderWithBulkAdd = new();
            foreach (IncomeEvent incomeEvent in incomeEvents)
            {
                summaryBuilder.AddIncomeEvent(incomeEvent);
            };
            summaryBuilderWithBulkAdd.AddIncomeEvents(incomeEvents);
            foreach (Expense expense in expenses)
            {
                summaryBuilder.AddExpense(expense);
            }
            summaryBuilderWithBulkAdd.AddExpenses(expenses);
            DateTime referenceDate = new(2020, 1, 5);
            summaryBuilder.SetReferenceDate(referenceDate);
            summaryBuilderWithBulkAdd.SetReferenceDate(referenceDate);
            summaryBuilder.SetTargetSavingRate(60);
            summaryBuilderWithBulkAdd.SetTargetSavingRate(60);
            List<Expense> fixedExpenses = new List<Expense>();
            for(int month = 1; month < 13; month++)
            {
                Expense rentExpense = new() { Date = new DateTime(2020, month, 1), Category = new ExpenseCategory { Name = "rent" }, Amount = 425m };
                Expense condoExpense = new() { Date = new DateTime(2020, month, 1), Category = new ExpenseCategory { Name = "condo" }, Amount = 35m };
                summaryBuilder.AddFixedExpense(rentExpense);
                summaryBuilderWithBulkAdd.AddFixedExpense(rentExpense);
                summaryBuilder.AddFixedExpense(condoExpense);
                summaryBuilderWithBulkAdd.AddFixedExpense(condoExpense);
                fixedExpenses.Add(rentExpense);
                fixedExpenses.Add(condoExpense);
            }
            // Builds the summary //
            AnnualSummary summary = summaryBuilder.Build();
            AnnualSummary summaryWithBulkAdd = summaryBuilderWithBulkAdd.Build();

            // Checks elements //
            Assert.Equal(incomeEvents.Count, summary.IncomeEvents.Count());
            Assert.Equal(incomeEvents.Count, summaryWithBulkAdd.IncomeEvents.Count());
            Assert.Equal(expenses.Count, summary.Expenses.Count);
            Assert.Equal(expenses.Count, summaryWithBulkAdd.Expenses.Count);
            Assert.Equal(fixedExpenses.Count, summary.FixedExpenses.Count);
            Assert.Equal(fixedExpenses.Count, summaryWithBulkAdd.FixedExpenses.Count);
            Assert.Equal(referenceDate, summary.ReferenceDate);
            Assert.Equal(referenceDate, summaryWithBulkAdd.ReferenceDate);
            Assert.Equal(60, summary.TargetSavingRate);
            Assert.Equal(60, summaryWithBulkAdd.TargetSavingRate);

            Assert.Equal(50.74m, summary.CurrentBalance, 2);
            Assert.Equal(50.74m, summaryWithBulkAdd.CurrentBalance, 2);
            Assert.Equal(21.15m, summary.DailyBudget, 2);
            Assert.Equal(21.15m, summaryWithBulkAdd.DailyBudget, 2);
            Assert.Equal(2.4m, summary.NeededDaysToRestoreBalance, 2);
            Assert.Equal(2.4m, summaryWithBulkAdd.NeededDaysToRestoreBalance, 2);
            Assert.Equal(1657.5m, summary.PlannedMonthlySavingAmount, 2);
            Assert.Equal(1657.5m, summaryWithBulkAdd.PlannedMonthlySavingAmount, 2);
            Assert.Equal(19890m, summary.PlannedAnnualSavingAmount, 2);
            Assert.Equal(19890m, summaryWithBulkAdd.PlannedAnnualSavingAmount, 2);
            Assert.Equal(19940.74m, summary.RealAnnualSavingAmount, 2);
            Assert.Equal(19940.74m, summaryWithBulkAdd.RealAnnualSavingAmount, 2);
            Assert.Equal(1661.73m, summary.RealMonthlySavingAmount, 2);
            Assert.Equal(1661.73m, summaryWithBulkAdd.RealMonthlySavingAmount, 2);
            Assert.Equal(7740.00m, summary.PlannedAnnualBudget, 2);
            Assert.Equal(7740.00m, summaryWithBulkAdd.PlannedAnnualBudget, 2);
            Assert.Equal(645.00m, summary.PlannedMonthlyBudget, 2);
            Assert.Equal(645.00m, summaryWithBulkAdd.PlannedMonthlyBudget, 2);
            Assert.Equal(9546m, summary.RealAnnualCostOfLiving, 2);
            Assert.Equal(9546m, summaryWithBulkAdd.RealAnnualCostOfLiving, 2);
            Assert.Equal(795.5m, summary.RealMonthlyCostOfLiving, 2);
            Assert.Equal(795.5m, summaryWithBulkAdd.RealMonthlyCostOfLiving, 2);

            Assert.Equal(15m, summary.ExpensesPerCategory["groceries"], 2);
            Assert.Equal(15m, summaryWithBulkAdd.ExpensesPerCategory["groceries"], 2);
            Assert.Equal(40m, summary.ExpensesPerCategory["car"], 2);
            Assert.Equal(40m, summaryWithBulkAdd.ExpensesPerCategory["car"], 2);

            Assert.Equal(55m, summary.ExpensesTotal, 2);
            Assert.Equal(55m, summaryWithBulkAdd.ExpensesTotal, 2);
            Assert.Equal(33150m, summary.TotalEarnings, 2);
            Assert.Equal(33150m, summaryWithBulkAdd.TotalEarnings, 2);
            Assert.Equal(new DateTime(2020, 1, 1), summary.StartingDate);
            Assert.Equal(new DateTime(2020, 1, 1), summaryWithBulkAdd.StartingDate);

            DateTime startingDate = new(2020, 1, 3);
            summaryBuilder.SetStartingDate(startingDate);
            summaryBuilderWithBulkAdd.SetStartingDate(startingDate);

            summary = summaryBuilder.Build();
            summaryWithBulkAdd = summaryBuilderWithBulkAdd.Build();

            Assert.Equal(8.44m, summary.CurrentBalance, 2);
            Assert.Equal(8.44m, summaryWithBulkAdd.CurrentBalance, 2);

            Assert.Equal(21.15m, summary.DailyBudget, 2);
            Assert.Equal(21.15m, summaryWithBulkAdd.DailyBudget, 2);

            Assert.Equal(0.4m, summary.NeededDaysToRestoreBalance, 2);
            Assert.Equal(0.4m, summaryWithBulkAdd.NeededDaysToRestoreBalance, 2);

            Assert.Equal(19898.44m, summary.RealAnnualSavingAmount, 2);
            Assert.Equal(19898.44m, summaryWithBulkAdd.RealAnnualSavingAmount, 2);
            Assert.Equal(1658.20m, summary.RealMonthlySavingAmount, 2);
            Assert.Equal(1658.20m, summaryWithBulkAdd.RealMonthlySavingAmount, 2);

            Assert.Equal(12230.00m, summary.RealAnnualCostOfLiving, 2);
            Assert.Equal(12230.00m, summaryWithBulkAdd.RealAnnualCostOfLiving, 2);
            Assert.Equal(1019.17m, summary.RealMonthlyCostOfLiving, 2);
            Assert.Equal(1019.17m, summaryWithBulkAdd.RealMonthlyCostOfLiving, 2);
        }
    }
}
