using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Confirmacion : ContentPage
    {
        private Constancia signPage;
        public Confirmacion(Constancia page)
        {
            InitializeComponent();
             signPage=page;
        }

        private async  void Button_Clicked(object sender, EventArgs e)
        {
           
            await Navigation.PopModalAsync(false);
            signPage.closeView(false);

        }
    }
}