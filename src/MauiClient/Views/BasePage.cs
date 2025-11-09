using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppMaui.Views
{
    public abstract class BasePage : ContentPage
    {
        public abstract void Build();

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            Build();
#if DEBUG
            HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);

#if DEBUG
            HotReloadService.UpdateApplicationEvent -= ReloadUI;
#endif
        }

        private void ReloadUI(Type[] obj)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Build();
            });
        }
    }
}
