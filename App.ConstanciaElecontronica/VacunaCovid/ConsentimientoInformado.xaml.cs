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

namespace App.ConstanciaElecontronica.VacunaCovid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsentimientoInformado : ContentPage
    {
        private readonly InicioVacuna _page;
        private readonly string Atencion;
        private readonly Vacuna Vacuna;
        
        private readonly bool DosisUnica;
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private readonly ServiceVacunacion _serviceFirma = new ServiceVacunacion(Secrets.APIConstancia);

        private bool isSamePatient;
        private bool isMenor;

        private string Razon;
        private readonly string token;
      

      
        public ConsentimientoInformado(InicioVacuna Page,string Atencion,bool isSamePatient, Vacuna Vacuna,bool DosisUnica,bool isMenor,string razon)
        {
            
            InitializeComponent();
            _page = Page;
            token = App.Current.Properties["token"].ToString();
            this.Atencion = Atencion;
            this.Vacuna = Vacuna;
            this.DosisUnica = DosisUnica;
             
            this.isSamePatient = isSamePatient;
            this.isMenor = isMenor;
            Razon = razon;



            if (this.isSamePatient)
            {
                bxvResponsable.IsVisible = false;
                bxvResponsable1.IsVisible = false;
                lytResponsable.IsVisible = false;


                lytFirmaAdd.IsVisible = false;
                bxvFirmaAdd.IsVisible = false;
                bxvFirmaAdd1.IsVisible = false;
            }
            else { 
                lytResponsable.IsVisible = true;
                if (!isMenor) {
                    lytFirmaAdd.IsVisible = false;
                    bxvFirmaAdd.IsVisible = false;
                    bxvFirmaAdd1.IsVisible = false;
                }
            }
            LoadDataPatiet();

          //  LoadPreguntas();
            //txtEnfermeria.Text = " " + App.Current.Properties["userName"].ToString()+" ";
            var currentDate = DateTime.Now;
            var currentText = $"CIUDAD: Bogotá FECHA: DIA {currentDate.ToString("dd")} MES {currentDate.ToString("MM")} AÑO {currentDate.ToString("yyyy")} HORA: {currentDate.ToString("hh:mm tt")}";
            txtDate.Text = currentText;

            if (!string.IsNullOrEmpty(Razon)) {
                lytRazon.IsVisible = true;
                txtRazon.Text = razon;
            }
        }

        

        private void LoadDataPatiet()
        {
            btnAceptar.IsEnabled = true;
            
            
            var response = _service.GetPatient(Atencion, token);
            if (response.code == (int)HttpStatusCode.OK)
            {
               
                var patient = response.data;
                lblNombres.Text = patient.NomCliente;
                lblApellidos.Text = patient.ApeCliente;
                lblDocumento.Text = patient.TipoDoc+" "+patient.NumDocumento;
                var currentDate = DateTime.Now;
                var age = currentDate.Year - patient.FecNacimiento.Year;
                // Go back to the year the person was born in case of a leap year
                if (patient.FecNacimiento.Date > currentDate.AddYears(-age)) age--;
                lblEdad.Text = " " + age + " años";
                lblFechaNacimiento.Text = "DIA " + patient.FecNacimiento.Day + " MES " + patient.FecNacimiento.Month + " AÑO" + patient.FecNacimiento.Year;
                lblEPS.Text = patient.Eps;
                lblVacuna.Text = " "+Vacuna.Nombre+" ";
                if (DosisUnica) {
                    lblDosDosis.Text = @"____";
                    lblDosisUnica.Text = " X ";
                }
                else {
                    lblDosDosis.Text = " X ";
                    lblDosisUnica.Text = @"____";
                } 

            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {
                /*lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;*/

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
                IdAtencion=int.Parse(Atencion)
            };

            

            if (!isSamePatient || isMenor)
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


           
            if (!cbxAceptar.IsChecked  && !cbxNoAceptar.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe marcar aceptar/no aceptar para continuar");
                return;
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
            //firma.Image = base64;
           
            firma.tipo = "VacunacionCovid19";
            var request = new RequestFirmaVacuna()
            {
                Firma = firma,
                LugarExpedicion = App.Current.Properties["lugarExp"].ToString(),
                FirmaVacunador = App.Current.Properties["sign"].ToString(),
                DosisUnica=DosisUnica,
                IdVacuna=Vacuna.IdVacuna,
                Acepta= cbxAceptar.IsChecked
                ,RazonNoFirma=Razon
            };
            if (isSamePatient)
                firma.Image = base64;
            /**/
            if (isMenor)
            {
                request.FirmaResponsable = base64;
                var base64II = string.Empty;
                Stream imageII = await sigPatientAdd.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
                if (imageII == null)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Debe firmar para continuar");
                    return;
                }
                byte[] bufferII = new byte[128 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = imageII.Read(bufferII, 0, bufferII.Length)) > 0)
                    {
                        ms.Write(bufferII, 0, read);
                    }
                    base64II = Convert.ToBase64String(bufferII);
                }
                firma.Image = base64II;
            }
            else
                if (!isSamePatient) {
                request.FirmaResponsable = base64;
            }
            
            var response = await _serviceFirma.SendFirma(request, token);
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
                _page.ClearForm();
            }

        }

        private void cbxAceptar_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxNoAceptar.IsChecked = !cbxAceptar.IsChecked;
        }

        private void cbxNoAceptar_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxAceptar.IsChecked = !cbxNoAceptar.IsChecked;
        }
    }
}