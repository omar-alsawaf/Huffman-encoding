using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System;
using System.Windows;

namespace FileCompression
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            App app = new App();
            MainWindow mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
    }
}
