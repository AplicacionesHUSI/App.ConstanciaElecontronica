using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.TratamientoFarmacologico
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsentimientoInformado : ContentPage
    {
        private readonly InicioTratamientoFarmacologico _page;
        private readonly string Atencion;
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private readonly ServiceTratamientoFarmacologico _serviceFirma = new ServiceTratamientoFarmacologico(Secrets.APIConstancia);

        private bool isSamePatient;
        
        private readonly string token;

        public ConsentimientoInformado(InicioTratamientoFarmacologico Page,string Atencion,bool isSamePatient,string Riesgos)
        {
            
            InitializeComponent();
            _page = Page;
            token = App.Current.Properties["token"].ToString();
            this.Atencion = Atencion;
            this.isSamePatient = isSamePatient;
            if (this.isSamePatient)
            {
                bxvResponsable.IsVisible = false;
                bxvResponsable1.IsVisible = false;
                lytResponsable.IsVisible = false;
            }
            else lytResponsable.IsVisible = true;
            LoadDataPatiet();

           
            txtRiesgos.Text = Riesgos;
            txtDoctor.Text = " "+App.Current.Properties["userName"].ToString();
            var currentDate = DateTime.Now;
            var currentText = $"Se firma en Bogotá, a los  {currentDate.ToString("dd")} dias del mes {currentDate.ToString("MM")} del año {currentDate.ToString("yyyy")}";
            txtDate.Text = currentText;
        }

       

        private void LoadDataPatiet()
        {
            btnAceptar.IsEnabled = true;
            var response = _service.GetPatient(Atencion, token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                lblAtencion.Text = Atencion;
                var patient = response.data;
                lblPatient.Text = patient.NomCliente + " " + patient.ApeCliente;
                lblPatient.TextColor = Color.Default;
                lblDocumento.Text = patient.TipoDoc+" "+patient.NumDocumento;
                
                lblSexo.Text = patient.IdSexo == 1 ? "Masculino" : "Femenino";
                var currentDate = DateTime.Now;
                var age = currentDate.Year - patient.FecNacimiento.Year;
                // Go back to the year the person was born in case of a leap year
                if (patient.FecNacimiento.Date > currentDate.AddYears(-age)) age--;
                lblEdad.Text = " " + age + " años";
            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;

            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            var firma = new FirmaPaciente()
            {
                IndMismoPaciente = isSamePatient,
                UsrWindowsReg = App.Current.Properties["name"].ToString(),
                IdAtencion=int.Parse(Atencion),
                tipo= "TratamientoCovid19"
            };
            if (!isSamePatient)
            {
                var validation = formFirma.Validation();
                if (!string.IsNullOrEmpty(validation))
                {
                    DependencyService.Get<IMessage>().ShortAlert(validation);
                    return;
                }
                firma.NomResponsable = formFirma.Nombre;
                firma.IdTipoDocResponsable = (byte)formFirma.IdTipoDoc;
                firma.NumDocResponsable = formFirma.NumDoc;
                firma.Parentesco = formFirma.Relacion;
            }

            //***
            Stream image = await sigPatient.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            if (image == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe firmar para continuar");
                return;
            }
            bar.IsRunning = true;
            btnAceptar.IsEnabled = false;
            
            var base64 = string.Empty;

            byte[] buffer = new byte[128 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = image.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                base64 = Convert.ToBase64String(buffer);
            }
            firma.Image = base64;

            var request = new RequestFirmaTratamiento()
            {
                Firma = firma,
                Riesgos = txtRiesgos.Text
            };


            var response = _serviceFirma.SendFirma(request, token);
            if (response.code == (int)HttpStatusCode.OK) await Navigation.PushModalAsync(new ConfirmacionConsentimiento(this), false);
            else DependencyService.Get<IMessage>().ShortAlert(response.message);
            btnAceptar.IsEnabled = true;
            bar.IsRunning = false;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            closeView(true);
        }
        public async void closeView(bool animated)
        {
            await Navigation.PopModalAsync(animated);
            if (!animated)
            {
                _page.Logout();                
            }

        }

       
    }
}