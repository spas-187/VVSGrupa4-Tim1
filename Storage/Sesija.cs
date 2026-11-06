using System;

namespace LoanManagement.Storage
{
    public class Sesija
    {
        public static int UserId = 0;
        public static string Username = "";
        public static string Role = "";
        public static string Token = "";

        public static string NapraviToken()
        {
            Random r = new Random();
            string slova = "abcdefghijklmnopqrstuvwxyz0123456789";
            string t = "";
            for (int i = 0; i < 16; i++)
            {
                t = t + slova[r.Next(0, slova.Length)];
            }
            return t;
        }

        public static bool JeLogovan()
        {
            if (Token == "")
            {
                return false;
            }
            return true;
        }

        public static void Odjava()
        {
            UserId = 0;
            Username = "";
            Role = "";
            Token = "";
        }
    }
}
