using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.CartaCompromisoCovid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmacionCartaComproCovid : ContentPage
    {
        private CartaCompromiso page;
        
        public ConfirmacionCartaComproCovid(CartaCompromiso page)
        {
            InitializeComponent();
            this.page = page;
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                // Do something
                Close();
                return false; // True = Repeat again, False = Stop the timer
            });
        }

        private async void Close()
        {

            await Navigation.PopModalAsync(false);
            page.closeView(false);

        }
    }
}