using System;
using System.Security.Cryptography;
using System.Text;

namespace LoanManagement.Storage
{
    public class Lozinke
    {
        public static string Hesiraj(string pass)
        {
            MD5 md5 = MD5.Create();
            byte[] bajti = Encoding.UTF8.GetBytes(pass);
            byte[] h = md5.ComputeHash(bajti);
            string rez = "";
            for (int i = 0; i < h.Length; i++)
            {
                rez = rez + h[i].ToString("x2");
            }
            return rez;
        }
    }
}
