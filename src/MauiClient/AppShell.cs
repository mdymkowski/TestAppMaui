using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppMaui.Views;

namespace TestAppMaui
{
    public sealed class AppShell : Shell
    {
        public AppShell() {
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

            Items.Add(new ShellContent
            {
                Title = "Main",
                ContentTemplate = new DataTemplate(typeof(MainPage)),
                Route = nameof(MainPage),
            });
        }
    }
}
