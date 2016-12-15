using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Anti_Analysis
{
    class Helpr
    {
        public static void Die(DetectionMethod m)
        {
            // TODO try catch maybe
            switch (m)
            {
                case DetectionMethod.FalseError:
                    {
                        MessageBox.Show("An unexpected error has occured and the application must quit. 0xF0010C0", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        Environment.Exit(0);
                        break;
                    }

                case DetectionMethod.Kill:
                    {
                        Process.GetCurrentProcess().Kill();
                        break;
                    }
                case DetectionMethod.ConsumeResources:
                    {
                        int i = 0;

                        while (i != 100000000)
                        {
                            i++;
                            Marshal.AllocHGlobal(i ^ 10);
                        }
                        break;
                    }
            }
        }
        public static IntPtr GetModuleHandle(string libName)
        {
            foreach (ProcessModule pMod in Process.GetCurrentProcess().Modules)
            {
                if (pMod.ModuleName.ToLower().Contains(libName.ToLower()))
                    return pMod.BaseAddress;
            }
            return IntPtr.Zero;
        }
        public static string CalculateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hash = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte t in hash)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string RandKey(int length, string letters = "abcdefghijklmnopqrstuvwxyz")
        {
            Random r = new Random(new Guid().GetHashCode());
            string text = string.Empty;
            char[] array = letters.ToCharArray();
            for (int i = 0; i <= length - 1; i++)
            {
                text += array[r.Next(0, array.Length)];
            }
            return text;
        }
    }
}
