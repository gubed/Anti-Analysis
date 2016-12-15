using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;


namespace Anti_Analysis.Modules
{
    class VirtualMachine : IAAM
    {

        private const long GB_50 = 50000000000;

        public override string Name => "Anti VirtualMachine";
        public override string Description => "Prevents file from running in a virtual machine";

        public override void DoWork(DetectionMethod m)
        {
            if (IsVirtualMachine())
                Helpr.Die(m);
               
        }
        // Tested & Working on 
        // - Malwr.com
        // - hybrid-analysis.com - VxStream
        // - sandbox.pikker.ee - Cuckoo sandbox
        // - TODO: sandbox.deepviz.com
        private bool IsVirtualMachine()
        {

            // Method One - main drive smaller than 50gb, likely a VM
            long driveSize = GetMainDriveSize();
            if (driveSize <= GB_50 * 2)
                return true;

            // Method Two - has common card of virtual machine
            if (HasVMCard())
                return true;

            // Method Three - checks for vm drivers
            if (HasVBOXDriver())
                return true;

            // Method Four - if machine has been on for less than 5 mins
            if (GetUptime() < TimeSpan.FromMinutes(5))
                return true;
                
            // Method Five - has VM mac address
            if (HasVMMac())
                return true;

            return false;
        }

        private bool HasVMMac()
        {
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            var macs = new[]
            {
                "00-05-69",
                "00:05:69",
                "000569",
                "00-50-56",
                "00:50:56",
                "005056",
                "00-0C-29",
                "00:0C:29",
                "000C29",
                "00-1C-14",
                "00:1C:14",
                "001C14",
                "08-00-27",
                "08:00:27",
                "080027",
            };
            foreach (string mac in macs)
            {
                if (mac == macAddr)
                    return true;
            }
            return false;
        }
        private bool HasVMCard()
        {
            try
            {
                string[] blacklist =
                {
                    "Standard VGA Graphics Adapter",
                    "Cirrus Logic 5446 Compatible Graphics Adapter"
                };

                using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                {
                    using (var items = searcher.Get())
                    {
                        foreach (var item in items)
                        {
                            string manufacturer = item["Manufacturer"].ToString().ToLower();

                            if (manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL")
                                || manufacturer.Contains("vmware")
                                || item["Model"].ToString() == "VirtualBox"
                                || manufacturer.ToLower().Contains("innotek gmbh") // VirtualBox    
                            )
                            {
                                return true;
                            }
                            foreach (string b in blacklist)
                            {
                                if (item["Description"].ToString() == b || item["Description"].ToString() == b.ToLower())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch { return true; } //TODO: may be messed up here
        }

        private TimeSpan GetUptime()
        {
            using (var uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue();       //Call this an extra time before reading its value
                return TimeSpan.FromSeconds(uptime.NextValue());
            }
        }
        // TODO: VMWare and other vms
        private bool HasVBOXDriver()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            var query = new SelectQuery("SELECT DeviceName from Win32_PnPSignedDriver");
            query.Condition = "DeviceName = 'VirtualBox Device'";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection moc = searcher.Get();

                if (moc.Count > 0)
                    return true;
            }

            query.Condition = "DeviceName = 'VirtualBox Graphics Adapter'";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection moc = searcher.Get();

                if (moc.Count > 0)
                    return true;
            }

            // 2 Seconds slower

            //using (var driveSearch = new ManagementObjectSearcher("Select DeviceName from Win32_PnPSignedDriver"))
            //{
            //    using (var items = driveSearch.Get())
            //    {
            //        foreach (var item in items)
            //        {
            //            var name = (string)item["DeviceName"];

            //            if (name == "VirtualBox Device")
            //            {
            //                s.Stop();
            //                    MessageBox.Show(s.ElapsedMilliseconds.ToString());
            //                return true;
            //            }

            //            if (name == "VirtualBox Graphics Adapter")
            //            {
            //                s.Stop();
            //                MessageBox.Show(s.ElapsedMilliseconds.ToString());
            //                return true;
            //            }
            //        }
            //    }
            //}
            return false;
        }
        private long GetMainDriveSize()
        {
            string mainDrive = Environment.SystemDirectory[0].ToString().ToLower();
            long byteSize = 0;

            using (var disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + mainDrive + ":\""))
            {
                disk.Get();
                byteSize = long.Parse(disk["Size"].ToString());
            }
            return byteSize;
        }
    }
}
