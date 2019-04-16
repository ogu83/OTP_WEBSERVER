using OtpNet;
using System;
using System.Text;
using System.Threading;

namespace OtpTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var secretKey = "1234567890123456";
            var secretBytes = Encoding.UTF8.GetBytes(secretKey);
            var myTOtp = new Totp(secretBytes, 10, OtpHashMode.Sha256, 6);

            for (int i = 0; i < 20; i++)
            {
                myTOtp = new Totp(secretBytes, 10, OtpHashMode.Sha256, 6);
                var newKey = myTOtp.ComputeTotp();
                Thread.Sleep(1000);
                Console.WriteLine(newKey);

                myTOtp = new Totp(secretBytes, 10, OtpHashMode.Sha256, 6);
                long timeLeft;
                var verify = myTOtp.VerifyTotp(newKey, out timeLeft);

                Console.WriteLine("TimeLeft : " + timeLeft / 3600);
                Console.WriteLine("Verify :" + verify);
            }

            Console.WriteLine("press any key to continue...");
            Console.ReadLine();
        }
    }
}
