using System;

namespace LoanManagement.Models
{
    public class LoanCalculationResult
    {
        public double Principal { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; }
        public double MonthlyPayment { get; set; }
        public int NumberOfMonths { get; set; }
        public double MonthlyInterestRate { get; set; }

        public override string ToString()
        {
            return "Principal: " + Principal + ", Rate: " + InterestRate + "%, Years: " + TermInYears +
                ", Months: " + NumberOfMonths + ", Monthly rate: " + MonthlyInterestRate + ", Payment: " + MonthlyPayment;
        }
    }
}
