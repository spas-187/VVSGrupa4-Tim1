using System;
using System.Collections.Generic;
using LoanManagement.Data;
using LoanManagement.Models;
using LoanManagement.Services.Customer;
using LoanManagement.Services.Loan;
using LoanManagement.Services.Repayment;
using LoanManagement.Storage;

namespace LoanManagement
{
    class Program
    {
        static DbClass db = new DbClass();
        static Baza baza = new Baza();
        static MailStub mail = new MailStub();

        static void Main(string[] args)
        {
            baza.Init();

            bool demo = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--demo")
                {
                    demo = true;
                }
            }
            if (demo == true)
            {
                Demo();
                return;
            }

            CustomerService cs = new CustomerService(db);
            string passwordPrompt = "Enter password: ";
            bool kraj = false;
            while (kraj == false)
            {
                Console.WriteLine();
                Console.WriteLine("=== Loan management ===");
                if (Sesija.JeLogovan() == true)
                {
                    Console.WriteLine("Logged in as: " + Sesija.Username + " (token " + Sesija.Token + ")");
                }
                Console.WriteLine("1. Register user");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Add customer");
                Console.WriteLine("4. Search customer by name");
                Console.WriteLine("5. Show loan details by id");
                Console.WriteLine("6. Delete customer by id");
                Console.WriteLine("7. Performance scenario");
                Console.WriteLine("0. Exit");
                Console.Write("Choice: ");
                string izbor = Console.ReadLine();

                if (izbor == null)
                {
                    kraj = true;
                }
                else if (izbor == "1")
                {
                    Console.Write("Username: ");
                    string u = Console.ReadLine();
                    Console.Write(passwordPrompt);
                    string p = Console.ReadLine();
                    int r = baza.RegistrujKorisnika(u, p);
                    if (r > 0)
                    {
                        Console.WriteLine("User registered");
                        mail.PosaljiDobrodoslicu(u + "@example.com");
                    }
                }
                else if (izbor == "2")
                {
                    Console.Write("Username: ");
                    string u = Console.ReadLine();
                    Console.Write(passwordPrompt);
                    string p = Console.ReadLine();
                    bool ok = baza.Prijava(u, p);
                    if (ok == true)
                    {
                        Console.WriteLine("Login ok, session token: " + Sesija.Token);
                    }
                    else
                    {
                        Console.WriteLine("Wrong username or password");
                    }
                }
                else if (izbor == "3")
                {
                    Console.Write("Name: ");
                    string n = Console.ReadLine();
                    Console.Write("Email: ");
                    string e = Console.ReadLine();
                    if (baza.EmailPostoji(e) == true)
                    {
                        Console.WriteLine("Customer with this email already exists");
                    }
                    else
                    {
                        try
                        {
                            int id = cs.AddCustomer(n, e);
                            baza.DodajKupca(id, n, e);
                            mail.PosaljiDobrodoslicu(e);
                            Console.WriteLine("Added customer " + id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                else if (izbor == "4")
                {
                    Console.Write("Name to search: ");
                    string n = Console.ReadLine();
                    string rez = baza.TraziKupce(n);
                    Console.WriteLine(rez);
                }
                else if (izbor == "5")
                {
                    if (Sesija.JeLogovan() == false)
                    {
                        Console.WriteLine("Please login first");
                    }
                    else
                    {
                        Console.Write("Loan id: ");
                        string id = Console.ReadLine();
                        Console.WriteLine(baza.DetaljiKredita(id));
                    }
                }
                else if (izbor == "6")
                {
                    if (Sesija.JeLogovan() == false)
                    {
                        Console.WriteLine("Please login first");
                    }
                    else
                    {
                        Console.Write("Customer id to delete: ");
                        string id = Console.ReadLine();
                        int br = baza.ObrisiKupca(id);
                        Console.WriteLine("Deleted rows: " + br);
                    }
                }
                else if (izbor == "7")
                {
                    PerformanceScenario();
                }
                else if (izbor == "0")
                {
                    kraj = true;
                }
                else
                {
                    Console.WriteLine("Unknown choice");
                }
            }
        }

        static void PerformanceScenario()
        {
            DbClass pdb = new DbClass();
            CustomerService cs = new CustomerService(pdb);
            LoanService ls = new LoanService(pdb);
            RepaymentService rs = new RepaymentService(pdb);

            int cid = cs.AddCustomer("Perf Customer", "perf@example.com");
            DateTime start = DateTime.Now.AddYears(-20);
            Loan l = new Loan(7001, cid, 100000, 5.5, 30, 567.79, start);
            pdb.AddLoan(l);
            for (int i = 0; i < 3000; i++)
            {
                Payment p = new Payment(i + 1, 7001, 25, start.AddDays(i * 2));
                pdb.AddPayment(p);
            }
            Console.WriteLine("Payments in memory: " + pdb.Payments.Count);

            DateTime t1 = DateTime.Now;
            double b = 0;
            for (int k = 0; k < 220; k++)
            {
                b = rs.CalculateRemainingBalance(7001);
            }
            DateTime t2 = DateTime.Now;
            TimeSpan razlika = t2 - t1;
            Console.WriteLine("Remaining balance: " + b);
            Console.WriteLine("Elapsed ms: " + razlika.TotalMilliseconds);
        }

        static void Demo()
        {
            DbClass ddb = new DbClass();
            CustomerService cs = new CustomerService(ddb);
            LoanService ls = new LoanService(ddb);
            RepaymentService rs = new RepaymentService(ddb);

            int cid = cs.AddCustomer("Demo Customer", "demo@example.com");
            Console.WriteLine(cs.GetCustomerDetails(cid));

            List<double> pr = new List<double>();
            List<double> ir = new List<double>();
            List<int> tr = new List<int>();
            pr.Add(25000);
            ir.Add(6.5);
            tr.Add(5);
            pr.Add(10000);
            ir.Add(8);
            tr.Add(2);
            List<LoanCalculationResult> rez = ls.CalculateMonthlyPayment(pr, ir, tr, 2);
            for (int i = 0; i < rez.Count; i++)
            {
                Console.WriteLine(rez[i]);
                Console.WriteLine("Total interest: " + ls.CalculateTotalInterest(rez[i].Principal, rez[i].MonthlyPayment, rez[i].TermInYears));
            }

            int lid = ls.AddLoan(cid, rez, DateTime.Now.AddMonths(-6));
            Console.WriteLine("Loan id: " + lid);
            rs.ProcessPayment(cid, 500, DateTime.Now.AddMonths(-3));
            Console.WriteLine("Remaining balance: " + rs.CalculateRemainingBalance(lid));

            try
            {
                rs.CalculateRemainingBalance(1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Caught: " + ex.Message);
            }
        }
    }
}
