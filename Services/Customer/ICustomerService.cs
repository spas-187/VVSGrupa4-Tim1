using System;

namespace LoanManagement.Services.Customer
{
    public interface ICustomerService
    {
        int AddCustomer(string name, string email);
        string GetCustomerDetails(int customerId);
    }
}
