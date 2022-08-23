using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static System.Threading.Mutex? mutex;

        //验证同名进程是否存在，防止多开
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, "EyeOfSauron");
            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("程序已经在运行！", "提示");
                this.Shutdown();
            }
        }
    }
}

