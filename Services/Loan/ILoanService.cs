using System;
using System.Collections.Generic;
using LoanManagement.Models;

namespace LoanManagement.Services.Loan
{
    public interface ILoanService
    {
        List<LoanCalculationResult> CalculateMonthlyPayment(List<double> principalAmounts, List<double> interestRates, List<int> loanTerms, int numberOfLoans);
        double CalculateTotalInterest(double principal, double monthlyPayment, int termInYears);
        int AddLoan(int customerId, List<LoanCalculationResult> loanResults, DateTime startDate);
    }
}
