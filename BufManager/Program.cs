﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BufManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form f = new Form1();
            Application.Run();
        }
        //static ;
    }
}
