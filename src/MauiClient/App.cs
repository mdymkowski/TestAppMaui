using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppMaui.Views;

namespace TestAppMaui
{
    public class App : Application
    {
        public App(AppShell appShell)
        {
            MainPage = appShell;
        }
    }
}