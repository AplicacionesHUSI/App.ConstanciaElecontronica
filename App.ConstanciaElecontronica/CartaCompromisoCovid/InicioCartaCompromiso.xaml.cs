using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
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

namespace App.ConstanciaElecontronica.CartaCompromisoCovid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioCartaCompromiso : ContentPage
    {
        private readonly ServiceCuidadosEnfermeria _serviceFirma = new ServiceCuidadosEnfermeria(Secrets.APIConstancia);
        private readonly ServicePatient _service;        
        private string Atencion = string.Empty;
        private bool Encontrado = false;
        private bool Aceptado = false;
        private readonly string token;

        private readonly ServiceCartaCompromiso _serviceCarta = new ServiceCartaCompromiso(Secrets.APIConstancia);
        private readonly ServiceInstruccionesEgreso _serviceInsEgreso = new ServiceInstruccionesEgreso(Secrets.APIConstancia);
        private readonly ServiceConfiguracion _serviceConf = new ServiceConfiguracion(Secrets.APIConstancia);

        public InicioCartaCompromiso(ServicePatient service)
        {
            InitializeComponent();
            _service = service;
            token = App.Current.Properties["token"].ToString();
          //  user.Text = "Inicio (" + App.Current.Properties["name"] + ")";
            ClearForm();
            lblOtroResponsable.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => mostrarResponsable2()),
            });
            formFirma.VerTelefono();
            formFirma2.VerTelefono();
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


            lytResponsable.IsVisible = false;
            bxvResponsable.IsVisible = false;
            bxvResponsable1.IsVisible = false;
            formFirma.Clear();
            lytResponsable2.IsVisible = false;
            bxvResponsable2.IsVisible = false;
            bxvResponsable21.IsVisible = false;
            formFirma2.Clear();
            cbxPaciente.IsChecked = false;
            cbxResponsable.IsChecked = false;
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

            var isPatienet = cbxPaciente.IsChecked;
            if (!isPatienet && !cbxResponsable.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar la persona que firma (paciente/responsable)");
                return;
            }

            var carta= new CartaCompromisoCovid19() { 
            IdAtencion=int.Parse(Atencion),
            IndMismoPaciente= isPatienet
            };

            if (!isPatienet)
            {
                var validation = formFirma.Validation();
                if (!string.IsNullOrEmpty(validation))
                {
                    DependencyService.Get<IMessage>().ShortAlert(validation);
                    return;
                }
                carta.NomResponsableUno = formFirma.Nombre;
                carta.IdTipoDocResponsableUno = (byte)formFirma.IdTipoDoc;
                carta.NumDocResponsableUno = formFirma.NumDoc;
                carta.ParentescoUno = formFirma.Relacion;
                carta.TelefonoUno = formFirma.Telefono;
                carta.IndAcompananteUno = true;
                if (responsable2) {

                    var validation2 = formFirma2.Validation();
                    if (!string.IsNullOrEmpty(validation2))
                    {
                        DependencyService.Get<IMessage>().ShortAlert(validation2);
                        return;
                    }
                    carta.NomResponsableDos = formFirma2.Nombre;
                    carta.IdTipoDocResponsableDos = (byte)formFirma2.IdTipoDoc;
                    carta.NumDocResponsableDos = formFirma2.NumDoc;
                    carta.ParentescoDos = formFirma2.Relacion;
                    carta.TelefonoDos = formFirma2.Telefono;
                    carta.IndAcompananteDos= true;
                }
            }


            if (Aceptado)
            {
                bool answer = await DisplayAlert("La carta de compromiso ya fue aceptada previamente.", "¿Desea continuar?", "Si", "No");
                if (!answer) return;

            }

            await Navigation.PushModalAsync(new CartaCompromiso(_service,_serviceCarta,_serviceConf,carta,this));
        }


        private void cbxResponsable_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxPaciente.IsChecked = !cbxResponsable.IsChecked;
            mostrarResponsable();
        }

        private void cbxPaciente_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            cbxResponsable.IsChecked = !cbxPaciente.IsChecked;
            mostrarResponsable();
        }

        private bool responsable2=false;
        private void mostrarResponsable2() {
            responsable2 = !responsable2;
            if (responsable2)
                lblOtroResponsable.Text = "Ocultar otro responsable";
            else
                lblOtroResponsable.Text = "Agregar otro responsable";
            lytResponsable2.IsVisible = responsable2;
            bxvResponsable2.IsVisible = responsable2;
            bxvResponsable21.IsVisible = responsable2;
                
            
        }

        private void mostrarResponsable()
        {
            if (cbxResponsable.IsChecked)
            {
                lytResponsable.IsVisible = true;
                bxvResponsable.IsVisible = true;
                bxvResponsable1.IsVisible = true;
            }
            else
            {
                lytResponsable.IsVisible = false;
                bxvResponsable.IsVisible = false;
                bxvResponsable1.IsVisible = false;
                responsable2 = true;
                mostrarResponsable2();
            }
        }
        private void validateSign()
        {
            var response = _serviceFirma.GetFirma(Atencion, "CartaCompromisoCovid", token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                Aceptado = true;
                txtValidation.IsVisible = true;
            }
        }
    }
}