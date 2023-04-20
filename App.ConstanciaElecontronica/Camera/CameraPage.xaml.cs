using App.ConstanciaElecontronica.HabeasData;
using App.ConstanciaElecontronica.InstruccionesEgreso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Camera
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {
        public static CameraPage CAMPage;
        private CartaCompromisoCovid.CartaCompromiso _form;
        private InstruEgresoFirma _formInstru;
        
        private ConstanciaHabeasDataEvent constanciaHabeasDataEvent;
        private ConstanciaHabeasData constanciaHabeasData;

        public CameraPage(CartaCompromisoCovid.CartaCompromiso cartaCompromiso)
        {
            InitializeComponent();
            CAMPage = this;
            _form = cartaCompromiso;
        }

        public CameraPage(InstruEgresoFirma InstruEgresoFirma)
        {
            InitializeComponent();
            CAMPage = this;
            _formInstru = InstruEgresoFirma;
        }

        public CameraPage(ConstanciaHabeasDataEvent constanciaHabeasDataEvent)
        {
            InitializeComponent();
            CAMPage = this;
            this.constanciaHabeasDataEvent = constanciaHabeasDataEvent;
        }

        public CameraPage(ConstanciaHabeasData constanciaHabeasData)
        {
            InitializeComponent();
            CAMPage = this;
            this.constanciaHabeasData = constanciaHabeasData;
        }

        public async void completePick() {


           
                Device.BeginInvokeOnMainThread( async () =>
                {
                    await Navigation.PopModalAsync(false);
                    if (_form != null)
                    await _form.enviarFirma();
                    if (_formInstru != null)
                        await _formInstru.SendFirma();
                    if (constanciaHabeasDataEvent != null)
                        await constanciaHabeasDataEvent.SendFirma();
                    if (constanciaHabeasData != null)
                        await constanciaHabeasData.SendFirma();
                    
                });
          

        }
    }
}