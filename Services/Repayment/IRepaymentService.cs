using System;

namespace LoanManagement.Services.Repayment
{
    public interface IRepaymentService
    {
        double CalculateRemainingBalance(int loanId);
        void ProcessPayment(int customerId, double amount, DateTime paymentDate);
    }
}
