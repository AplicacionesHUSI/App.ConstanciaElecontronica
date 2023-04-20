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
    public partial class ConfirmacionHabeasData : ContentPage
    {
        private ConstanciaHabeasData SignPage;
        public ConfirmacionHabeasData(ConstanciaHabeasData page)
        {
            InitializeComponent();
            SignPage = page;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo("es");
            //volver
            await Navigation.PopModalAsync(false);
            SignPage.closeView(false);
        }
    }
}