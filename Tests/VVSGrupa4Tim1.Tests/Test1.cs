using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoanManagement.Services.Loan;
using LoanManagement.Data;
using LoanManagement.Services.Customer;

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
    [TestClass]
    public class CustomerServiceDynamicDataTests
    {
        [TestMethod]
        [DynamicData(nameof(CustomerCsvData), DynamicDataSourceType.Method)]
        public void AddCustomer_ValidData_AddsCustomer(
            string name,
            string email)
        {
            var db = new DbClass();
            var service = new CustomerService(db);

            int id = service.AddCustomer(name, email);

            Assert.AreEqual(1, db.Customers.Count);
            Assert.AreEqual(id, db.Customers[0].Id);
            Assert.AreEqual(name, db.Customers[0].Name);
            Assert.AreEqual(email, db.Customers[0].Email);
        }

        [TestMethod]
        [DynamicData(nameof(CustomerXmlData), DynamicDataSourceType.Method)]
        public void AddCustomer_ValidData_ReturnsGeneratedId(
            string name,
            string email)
        {
            var db = new DbClass();
            var service = new CustomerService(db);

            int id = service.AddCustomer(name, email);

            Assert.IsTrue(id >= 1000 && id <= 9998);
            Assert.AreEqual(id, db.Customers[0].Id);
        }

        public static IEnumerable<object[]> CustomerCsvData()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "TestData", "customers.csv");
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                string[] fields = line.Split(',');
                if (fields.Length != 2)
                {
                    throw new FormatException("CSV red mora sadržavati ime i email.");
                }

                yield return new object[] { fields[0].Trim(), fields[1].Trim() };
            }
        }

        public static IEnumerable<object[]> CustomerXmlData()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "TestData", "customers.xml");
            XDocument document = XDocument.Load(path);

            foreach (XElement customer in document.Descendants("Customer"))
            {
                string name = (string)customer.Element("Name")
                    ?? throw new FormatException("XML kupac nema Name element.");
                string email = (string)customer.Element("Email")
                    ?? throw new FormatException("XML kupac nema Email element.");

                yield return new object[] { name, email };
            }
        }
    }

    [TestClass]
    public class MoqCustomerServiceTests
    {
        [TestMethod]
        public void AddCustomer_MockReturnsConfiguredId()
        {
            var mock = new Mock<ICustomerService>();
            mock.Setup(service => service.AddCustomer("Ada Lovelace", "ada@example.com"))
                .Returns(4242);

            ICustomerService service = mock.Object;
            int id = service.AddCustomer("Ada Lovelace", "ada@example.com");

            Assert.AreEqual(4242, id);
            mock.Verify(service => service.AddCustomer("Ada Lovelace", "ada@example.com"), Times.Once);
        }

        [TestMethod]
        public void GetCustomerDetails_MockReturnsConfiguredDetails()
        {
            const int customerId = 1234;
            const string expected = "Id: 1234, Name: Ada Lovelace, Email: ada@example.com";
            var mock = new Mock<ICustomerService>();
            mock.Setup(service => service.GetCustomerDetails(customerId))
                .Returns(expected);

            ICustomerService service = mock.Object;
            string details = service.GetCustomerDetails(customerId);

            Assert.AreEqual(expected, details);
            mock.Verify(service => service.GetCustomerDetails(customerId), Times.Once);
        }
    }
}