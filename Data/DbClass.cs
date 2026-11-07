using System;
using System.Collections.Generic;
using LoanManagement.Models;

namespace LoanManagement.Data
{
    public class DbClass
    {
        public List<Customer> Customers { get; set; }
        public List<Loan> Loans { get; set; }
        public List<Payment> Payments { get; set; }

        public DbClass()
        {
            Customers = new List<Customer>();
            Loans = new List<Loan>();
            Payments = new List<Payment>();
        }

        public void AddCustomer(Customer c)
        {
            Customers.Add(c);
        }

        public void AddLoan(Loan l)
        {
            Loans.Add(l);
        }

        public void AddPayment(Payment p)
        {
            Payments.Add(p);
        }

        public Customer GetCustomer(int id)
        {
            Customer rez = null;
            for (int i = 0; i < Customers.Count; i++)
            {
                if (Customers[i].Id == id)
                {
                    rez = Customers[i];
                }
            }
            return rez;
        }

        public Loan GetLoan(int id)
        {
            Loan rez = null;
            for (int i = 0; i < Loans.Count; i++)
            {
                if (Loans[i].Id == id)
                {
                    rez = Loans[i];
                }
            }
            return rez;
        }

        public List<Payment> GetPaymentsForLoan(int loanId)
        {
            List<Payment> rez = new List<Payment>();
            List<Payment> tmp = new List<Payment>();
            foreach (Payment p in Payments)
            {
                tmp.Add(p);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                if (tmp[i].Id == loanId)
                {
                    rez.Add(tmp[i]);
                }
            }
            return rez;
        }
    }
}
