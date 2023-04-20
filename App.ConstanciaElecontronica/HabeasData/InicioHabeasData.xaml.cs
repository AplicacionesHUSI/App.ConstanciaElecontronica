using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Process;
using App.ConstanciaElecontronica.Services;
using Plugin.Multilingual;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.HabeasData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioHabeasData : ContentPage
    {
        private readonly ServicePatient _service;

        private  int TipoDoc = 0;
        private  string Documento = string.Empty;
        private bool Encontrado = false;
        private readonly string token;
        public InicioHabeasData(ServicePatient service)
        {
            InitializeComponent();
            _service = service;
            token=App.Current.Properties["token"].ToString();
         //   user.Text = "Inicio (" + App.Current.Properties["name"] + ")";
            ClearForm();
            ClearFormHead();
        }
        public void ClearFormHead() {
            cbxTipoDoc.SelectedIndex = 1;
            txtDocumento.Text = "";
        }

        public void ClearForm()
        {
            //**

           
            lytAceptar.IsVisible = false;
            lytPaciente.IsVisible = false;
            lytButtons.IsVisible = false;
            
            BoxExternal1.IsVisible = false;
            BoxInternal1.IsVisible = false;
            
            BoxExternal.IsVisible = false;
            BoxInternal.IsVisible = false;
            Encontrado = false;
            btnContinuar.IsEnabled = true;
            lblHabeas.IsVisible = false;
            lblPatient.Text = "";
            lblDocumento.Text = "";
            lblTipoDoc.Text = "";
            lblSexo.Text = "";
            lblEdad.Text = "";
            

            cbxPaciente.CheckedChanged -= cbxPaciente_CheckedChanged_1;
            cbxResponsable.CheckedChanged -= cbxResponsable_CheckedChanged_1;

            cbxPaciente.IsChecked = false;
            cbxResponsable.IsChecked = false;
            cbxPaciente.IsEnabled = true;
            cbxResponsable.IsEnabled = true;
            cbxPaciente.CheckedChanged += cbxPaciente_CheckedChanged_1;
            cbxResponsable.CheckedChanged += cbxResponsable_CheckedChanged_1;

            cbxSpanish.CheckedChanged -= cbxSpanish_CheckedChanged;
            cbxEnglish.CheckedChanged -= cbxEnglish_CheckedChanged;

            cbxSpanish.IsChecked = false;
            cbxEnglish.IsChecked = false;
            cbxSpanish.CheckedChanged += cbxSpanish_CheckedChanged;
            cbxEnglish.CheckedChanged += cbxEnglish_CheckedChanged;
            indHabeas = false;

        }

        private void BtnLogoutClick(object sender, EventArgs e)
        {
            App.Current.Logout();
        }

        private void BtnDocClicked(object sender, EventArgs e)
        {

            var doc = txtDocumento.Text;
            
            if (string.IsNullOrEmpty(doc)) {
                DependencyService.Get<IMessage>().ShortAlert("El documento no puede ser vacio");
                return ;
            }
            int selectedIndex = cbxTipoDoc.SelectedIndex;
            if (selectedIndex==-1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe elegir un tipo de documento");
                return;
            }

            
            var tipoDoc = (string)cbxTipoDoc.ItemsSource[selectedIndex];
            var IdTipoDoc = Util.Util.GetIdDoc(tipoDoc);
            TipoDoc = IdTipoDoc;
            Documento = doc;
            LoadPatient(IdTipoDoc, doc);
        }

        private bool indHabeas = false;

        private void LoadPatient(int IdtipoDoc,string documento)
        {
            ClearForm();
            
            var response = _service.GetPatient(IdtipoDoc, documento, token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                var patient = response.data;
                lblPatient.Text = patient.NomCliente + " " + patient.ApeCliente;
                lblPatient.TextColor = Color.Default;
                lblDocumento.Text = patient.NumDocumento;
                lblTipoDoc.Text = patient.TipoDoc;
                lblSexo.Text = patient.IdSexo == 1 ? "Masculino" : "Femenino";
                var currentDate = DateTime.Now;
                var age = currentDate.Year - patient.FecNacimiento.Year;
                // Go back to the year the person was born in case of a leap year
                if (patient.FecNacimiento.Date > currentDate.AddYears(-age)) age--;
                lblEdad.Text = " " + age + " años";
                Encontrado = true;
                indHabeas = patient.IndHabeasData != null ? (bool) patient.IndHabeasData : false;
                lblHabeas.IsVisible = false;
                if (indHabeas)              

                    lblHabeas.IsVisible = true;
                    /*btnContinuar.IsEnabled = false;
                    cbxPaciente.IsEnabled = false;
                    lytPaciente.IsVisible = true;
                    lytAceptar.IsVisible = true;*/
           
        
                btnContinuar.IsEnabled = true;
                cbxPaciente.IsEnabled = true;
                lytAceptar.IsVisible = true;
                lytPaciente.IsVisible = true;
                lytButtons.IsVisible = true;
               
                BoxExternal1.IsVisible = true;
                BoxInternal1.IsVisible = true;

                BoxExternal.IsVisible = true;
                BoxInternal.IsVisible = true;
                
                

            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {
                lytPaciente.IsVisible = true;
                lytAceptar.IsVisible = true;
                btnContinuar.IsEnabled = false;
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                Encontrado = false;
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
            if (response.code == (int)HttpStatusCode.InternalServerError)
            {
                DependencyService.Get<IMessage>().ShortAlert(
                                ((response.message != null) && (response.message.Length > 0)) ?
                                  response.message : "Paciente no encontrado");

                lytPaciente.IsVisible = true;
                lytAceptar.IsVisible = true;
                btnContinuar.IsEnabled = false;
                lblPatient.Text = "Paciente no encontrado";
                lblPatient.TextColor = Color.DarkRed;
                Encontrado = false;
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (!Encontrado) {
                DependencyService.Get<IMessage>().ShortAlert("Debe buscar un paciente.");
                return;
            }
            var spanish = cbxSpanish.IsChecked;

            if (!spanish && !cbxEnglish.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar un idioma.");
                return;
            }
            var isPatienet = cbxPaciente.IsChecked;
            if (!isPatienet && !cbxResponsable.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar la persona que firma (paciente/responsable)");
                return;
            }
            
            var languaje = string.Empty;
            if (spanish)
                languaje = "es";
            else
                languaje = "en";

            if (indHabeas) { 
                bool answer = await DisplayAlert("El habeas data ya fue aceptado", "¿Desea continuar?", "Si", "No");
                if (!answer) return;
            }


            CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo(languaje);
            await Navigation.PushModalAsync(new ConstanciaHabeasData(TipoDoc,Documento, isPatienet, this,languaje));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ClearFormHead();
            ClearForm();
        }

        private void cbxPaciente_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxResponsable.IsChecked = !cbxPaciente.IsChecked;
        }

        private void cbxResponsable_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxPaciente.IsChecked = !cbxResponsable.IsChecked;
        }

        private void cbxSpanish_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxEnglish.IsChecked = !cbxSpanish.IsChecked;
        }

        private void cbxEnglish_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxSpanish.IsChecked = !cbxEnglish.IsChecked;
        }

        private void cbxTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDocumento.Keyboard = (cbxTipoDoc.SelectedIndex == 2 || cbxTipoDoc.SelectedIndex == 5 || cbxTipoDoc.SelectedIndex == 4
    || cbxTipoDoc.SelectedIndex == 9 || cbxTipoDoc.SelectedIndex == 11) ? Keyboard.Default : Keyboard.Numeric;

        }
    }
}