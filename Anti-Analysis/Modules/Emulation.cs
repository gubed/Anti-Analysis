using System.Diagnostics;
using System.Windows.Forms;

namespace Anti_Analysis.Modules
{
    class Emulation : IAAM
    {

        public override string Name => "Anti Emulation";
        public override string Description => "Prevents analysis programs from emulating the target application by delaying the application start with MD5 cracking.";

        public override void DoWork(DetectionMethod m)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            bool cracked = false;
            // TODO: Improve random key and md5 methods
            // TODO: Improve calculation size based on computer specs (for more accurate timing)
            // Currently ~ 58 seconds
            try
            {
                string letters = "abcdefghijklmnopqrstuvwxyz";
                char[] a = letters.ToCharArray();
                string shit = Helpr.RandKey(4) + Helpr.RandKey(1, "abcd"); //TODO adjust time here.
                string md5 = Helpr.CalculateMD5Hash(shit);

                for (int i = 0; i < letters.Length; i++)
                {
                    for (int j = 0; j < letters.Length; j++)
                    {
                        for (int k = 0; k < letters.Length; k++)
                        {
                            for (int l = 0; l < letters.Length; l++)
                            {
                                for (int mm = 0; mm < letters.Length; mm++)
                                {
                                   // for (int n = 0; n < 6; n++)
                                    {
                                        string letterOne = a[i].ToString();
                                        string letterTwo = a[j].ToString();
                                        string letterThree = a[k].ToString();
                                        string letterFour = a[l].ToString();
                                        string letterFive = a[mm].ToString();
                                      //  string letterSix = a[n].ToString();

                                        string stry = Helpr.CalculateMD5Hash(letterOne + letterTwo + letterThree + letterFour + letterFive );
                                        if (stry == md5)
                                        {
                                            cracked = true;
                                            s.Stop();
                                            MessageBox.Show(s.ElapsedMilliseconds.ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                
            }
            catch
            {
                if (!cracked)
                {
                    Helpr.Die(m);
                }
            }
        }

    }
}
