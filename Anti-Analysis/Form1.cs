using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Forms;
using Anti_Analysis.Modules;

namespace Anti_Analysis
{
    public partial class Form1 : Form
    {
        private List<IAAM> mods = new List<IAAM>();

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadModules()
        {
           // mods.Add(new Emulation());
            //mods.Add(new Sandboxie());
            mods.Add(new VirtualMachine());
        }

        private void RunModules()
        {
            foreach (var a in mods)
                a.DoWork(DetectionMethod.FalseError);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            //ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver");

            //ManagementObjectCollection objCollection = objSearcher.Get();

            //foreach (ManagementObject obj in objCollection)
            //{
            //    string info = String.Format("Device='{0}',Manufacturer='{1}',DriverVersion='{2}' ", obj["DeviceName"], obj["Manufacturer"], obj["DriverVersion"]);
            //    if (info.ToLower().Contains("virtual"))
            //        MessageBox.Show(info);
            //}

            LoadModules();

            RunModules();
        }
        
    }

    enum DetectionMethod
    {
        FalseError,
        Kill,
        ConsumeResources
    }
}
