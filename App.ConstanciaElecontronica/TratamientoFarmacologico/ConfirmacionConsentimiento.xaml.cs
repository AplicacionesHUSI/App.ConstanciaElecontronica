using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.TratamientoFarmacologico
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmacionConsentimiento : ContentPage
    {
        private readonly ConsentimientoInformado _page;
        public ConfirmacionConsentimiento(ConsentimientoInformado Page)
        {
            InitializeComponent();
            _page = Page;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
            _page.closeView(false);
        }
    }
}