using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoanManagement.Services.Loan;

namespace LoanManagement.Tests
{
    [TestClass]
    public class LoanServiceTests
    {
        [TestMethod]
        public void CalculateMonthlyPayment_ValidLoan_ReturnsExpectedMonthlyPayment()
        {
            var service = new LoanService(null);

            var result = service.CalculateMonthlyPayment(
                new List<double> { 12000 },
                new List<double> { 12 },
                new List<int> { 1 },
                1);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(12000, result[0].Principal);
            Assert.AreEqual(12, result[0].NumberOfMonths);
            Assert.AreEqual(1066.19, result[0].MonthlyPayment, 0.01);
        }

        [TestMethod]
        public void CalculateMonthlyPayment_PrincipalIsNotPositive_ThrowsException()
        {
            var service = new LoanService(null);

            try
            {
                service.CalculateMonthlyPayment(
                    new List<double> { 0 },
                    new List<double> { 12 },
                    new List<int> { 1 },
                    1);

                Assert.Fail("Očekivan je InvalidOperationException.");
            }
            catch (InvalidOperationException)
            {
                // Test prolazi: metoda je bacila očekivani izuzetak.
            }
        }

        [TestMethod]
        public void CalculateMonthlyPayment_PrincipalListHasWrongSize_ThrowsException()
        {
            var service = new LoanService(null);

            try
            {
                service.CalculateMonthlyPayment(
                    new List<double> { 10000, 20000 },
                    new List<double> { 12 },
                    new List<int> { 1 },
                    1);
                Assert.Fail("Očekivan je ArgumentException.");
            }
            catch (ArgumentException)
            {
                // Test prolazi: metoda je bacila očekivani izuzetak.
            }
        }

        [TestMethod]
        public void CalculateTotalInterest_ReturnsExpectedInterest()
        {
            var service = new LoanService(null);

            double kamata = service.CalculateTotalInterest(
                principal: 10000,
                monthlyPayment: 458.3333333333,
                termInYears: 2);

            Assert.AreEqual(1000, kamata, 0.01);
        }

    }
}