using System;

namespace LoanManagement.Storage
{
    public class MailStub
    {
        private string apiKljuc = "demo-mail-key-0000-0000";
        private string url = "https://mail.example.invalid/send";

        public string PosaljiDobrodoslicu(string email)
        {
            string header = "Authorization: Bearer " + apiKljuc;
            string tijelo = "to=" + email + "&text=Welcome";
            string poruka = "POST " + url + " [" + header + "] " + tijelo;
            // nothing is sent, only prepared
            Console.WriteLine("Welcome mail prepared for " + email);
            return poruka;
        }
    }
}
