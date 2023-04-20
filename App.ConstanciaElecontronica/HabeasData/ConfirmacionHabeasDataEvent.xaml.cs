using Plugin.Multilingual;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.HabeasData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmacionHabeasDataEvent : ContentPage
    {
        private ConstanciaHabeasDataEvent SignPage;
        public ConfirmacionHabeasDataEvent(ConstanciaHabeasDataEvent page)
        {
            InitializeComponent();
            SignPage = page;
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                // Do something
                close();
                return false; // True = Repeat again, False = Stop the timer
            });
        }

        private async void close()
        {
            CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo("es");
            //volver
            await Navigation.PopModalAsync(false);
            SignPage.closeView(false);
        }
    }
}