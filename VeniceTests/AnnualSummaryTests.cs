using System;
using System.Collections.Generic;
using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class AnnualSummaryTests
    {
        [Fact]
        public void TestAnnualSummary()
        {
            List<Paycheck> paychecks = new()
            {
                new Paycheck { Year = 2020, Month = 1, NetSalary = 2100 },
                new Paycheck { Year = 2020, Month = 2, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 3, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 4, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 5, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 6, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 7, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 8, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 9, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 10, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 11, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 12, NetSalary = 2600 },
                new Paycheck { Year = 2020, Month = 13, NetSalary = 2400 }
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
            foreach (Paycheck paycheck in paychecks)
            {
                summaryBuilder.AddPaycheck(paycheck);
            };
            summaryBuilderWithBulkAdd.AddPaychecks(paychecks);
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
                Expense rentExpense = new Expense() { Date = new DateTime(2020, month, 1), Category = new ExpenseCategory { Name = "rent" }, Amount = 425m };
                Expense condoExpense = new Expense() { Date = new DateTime(2020, month, 1), Category = new ExpenseCategory { Name = "condo" }, Amount = 35m };
                summaryBuilder.AddFixedExpense(rentExpense);
                summaryBuilderWithBulkAdd.AddFixedExpense(rentExpense);
                summaryBuilder.AddFixedExpense(condoExpense);
                summaryBuilderWithBulkAdd.AddFixedExpense(condoExpense);
                fixedExpenses.Add(rentExpense);
                fixedExpenses.Add(condoExpense);
            }
            summaryBuilder.AddGain(new DateTime(2020, 2, 1), 50m);
            summaryBuilderWithBulkAdd.AddGain(new DateTime(2020, 2, 1), 50m);
            // Builds the summary //
            AnnualSummary summary = summaryBuilder.Build();
            AnnualSummary summaryWithBulkAdd = summaryBuilderWithBulkAdd.Build();

            // Checks elements //
            Assert.Equal(paychecks, summary.Paychecks);
            Assert.Equal(paychecks, summaryWithBulkAdd.Paychecks);
            Assert.Equal(expenses, summary.Expenses);
            Assert.Equal(expenses, summaryWithBulkAdd.Expenses);
            Assert.Equal(fixedExpenses, summary.FixedExpenses);
            Assert.Equal(fixedExpenses, summaryWithBulkAdd.FixedExpenses);
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

            Assert.Equal(2100, summary.EarningsPerMonth[1]);
            Assert.Equal(2650, summary.EarningsPerMonth[2]);
            Assert.Equal(2600, summary.EarningsPerMonth[3]);
            Assert.Equal(2600, summary.EarningsPerMonth[4]);
            Assert.Equal(2600, summary.EarningsPerMonth[5]);
            Assert.Equal(2600, summary.EarningsPerMonth[6]);
            Assert.Equal(2600, summary.EarningsPerMonth[7]);
            Assert.Equal(2600, summary.EarningsPerMonth[8]);
            Assert.Equal(2600, summary.EarningsPerMonth[9]);
            Assert.Equal(2600, summary.EarningsPerMonth[10]);
            Assert.Equal(2600, summary.EarningsPerMonth[11]);
            Assert.Equal(5000, summary.EarningsPerMonth[12]);

            Assert.Equal(2100, summaryWithBulkAdd.EarningsPerMonth[1]);
            Assert.Equal(2650, summaryWithBulkAdd.EarningsPerMonth[2]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[3]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[4]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[5]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[6]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[7]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[8]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[9]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[10]);
            Assert.Equal(2600, summaryWithBulkAdd.EarningsPerMonth[11]);
            Assert.Equal(5000, summaryWithBulkAdd.EarningsPerMonth[12]);

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
