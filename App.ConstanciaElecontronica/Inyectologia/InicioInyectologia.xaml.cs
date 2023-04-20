using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Firma;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Inyectologia
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioInyectologia : ContentPage
    {
        private readonly ServiceCuidadosEnfermeria _serviceFirma = new ServiceCuidadosEnfermeria(Secrets.APIConstancia);
        private readonly ServicePatient _service;        
        private string Atencion = string.Empty;
        private bool Encontrado = false;
        private bool Aceptado = false;
        private readonly string token;

        public InicioInyectologia(ServicePatient service)
        {
            InitializeComponent();
            _service = service;
            token = App.Current.Properties["token"].ToString();
          //  user.Text = "Inicio (" + App.Current.Properties["name"] + ")";
            ClearForm();
            
        }

       
        public void ClearForm()
        {
            txtValidation.IsVisible = false;
            Encontrado = false;
            Aceptado = false;

            lytButtons.IsVisible = false;
            lytPatient.IsVisible = false;
         
            boxExternal1.IsVisible = false;
            boxInternal1.IsVisible = false;

            Atencion = null;
            txtAtencion.Text = "";
        }

        void BtnLogoutClick(object sender, EventArgs e)
        {
            App.Current.Logout();
        }

        private void BtnLimpiarClicked(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnAtencion(object sender, EventArgs e)
        {
            var atencion = txtAtencion.Text;
            if (string.IsNullOrEmpty(atencion)) {
                DependencyService.Get<IMessage>().ShortAlert("El numero de atención no puede ser vacio");
                return;
            }

            loadPatietn(atencion);

        }

        private void loadPatietn(string atencion)
        {
            txtValidation.IsVisible = false;
            Aceptado = false;
            Encontrado = false;
            Atencion = atencion;
            var response = _service.GetPatient(Atencion, token);
            showLayouts();
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
                validateSign();
            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {               
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                lblDocumento.Text = "";
                lblTipoDoc.Text = "";
                lblSexo.Text = "";
                lblEdad.Text = "";
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
            if (response.code == (int)HttpStatusCode.InternalServerError)
            {
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                DependencyService.Get<IMessage>().ShortAlert(response.message);                
         
            }
        }

        private void showLayouts()
        {
            lytButtons.IsVisible = true;
            lytPatient.IsVisible = true;
          
            boxExternal1.IsVisible = true;
            boxInternal1.IsVisible = true;
       
        }


        private async void BtnContinuar(object sender, EventArgs e)
        {

            if (!Encontrado) {
                DependencyService.Get<IMessage>().ShortAlert("Debe buscar un paciente con atención activa.");
                return;
            }
            if (Aceptado)
            {
                DependencyService.Get<IMessage>().ShortAlert("Ya se acepto previamente el cosentimiento.");
                return;
            }

            var isPatienet = cbxPaciente.IsChecked;
            if (!isPatienet && !cbxResponsable.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar la persona que firma (paciente/responsable)");
                return;
            }
            await Navigation.PushModalAsync(new ConsentimientoInformado(this,Atencion, isPatienet));
        }


        private void cbxResponsable_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxPaciente.IsChecked = !cbxResponsable.IsChecked;
        }

        private void cbxPaciente_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxResponsable.IsChecked = !cbxPaciente.IsChecked;
        }
        private void validateSign()
        {
            var response = _serviceFirma.GetFirma(Atencion, "Inyectologia", token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                Aceptado = true;
                txtValidation.IsVisible = true;
            }
        }
    }
}