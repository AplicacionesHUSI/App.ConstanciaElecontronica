using App.ConstanciaElecontronica.Camera;
using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Services;
using Plugin.Multilingual;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.HabeasData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConstanciaHabeasData : ContentPage
    {
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private bool isSamePatient;
        private readonly InicioHabeasData _inicio;
        private readonly string token;
        private readonly string _languaje;
        private FirmaHabeasData firma;
        public ConstanciaHabeasData(int TipoDoc,string NumDoc,bool isSamePatient,InicioHabeasData inicio, string languaje)
        {
            this.isSamePatient = isSamePatient;
            _inicio = inicio;
            InitializeComponent();
            token = App.Current.Properties["token"].ToString();
            if (isSamePatient)
            {
                bxvResponsable.IsVisible = false;
                bxvResponsable1.IsVisible = false;
                lytResponsable.IsVisible = false;
            }
            else lytResponsable.IsVisible = true;
            _languaje = languaje;
            LoadDataPatiet(TipoDoc, NumDoc);
            
        }
        private int IdCliente =0;
        private ServiceInstruction _serviceFirma= new ServiceInstruction(Secrets.APIConstancia);

        private void LoadDataPatiet(int tipoDoc, string numDoc)
        {
            btnAceptar.IsEnabled = true;
            var response = _service.GetPatient(tipoDoc, numDoc, token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                
                var patient = response.data;
                IdCliente = patient.IdCliente;
                lblPatient.Text = patient.NomCliente + " " + patient.ApeCliente;
                lblPatient.TextColor = Color.Default;
                lblDocumento.Text = patient.NumDocumento;
                lblTipoDoc.Text = patient.TipoDoc;
                //
                if (_languaje=="es")
                lblSexo.Text = patient.IdSexo == 1 ? "Masculino" : "Femenino";
                else
                    lblSexo.Text = patient.IdSexo == 1 ? "Male" : "Female";
                var currentDate = DateTime.Now;
                var age = currentDate.Year - patient.FecNacimiento.Year;
                // Go back to the year the person was born in case of a leap year
                if (patient.FecNacimiento.Date > currentDate.AddYears(-age)) age--;
                //
                if (_languaje == "es")
                    lblEdad.Text = " " + age + " años";
                else
                    lblEdad.Text = " " + age + " years";
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
        public async void closeView(bool animated)
        {
            await Navigation.PopModalAsync(animated);
            if (!animated) {
                _inicio.ClearForm();
                _inicio.ClearFormHead();
            }
            
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo("es");
            closeView(true);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            
            
            firma = new FirmaHabeasData()
            {
                IdCliente = IdCliente,
                IndMismoPaciente = isSamePatient,
                UsrWindowsReg = App.Current.Properties["name"].ToString()
            };
            if (!isSamePatient)
            {
                var validation = formFirma.Validation();
                if (!string.IsNullOrEmpty(validation))
                {
                    DependencyService.Get<IMessage>().ShortAlert(validation);
                    return;
                }
                firma.Responsable = formFirma.Nombre;
                firma.IdTipoDocResponsable = (byte)formFirma.IdTipoDoc;
                firma.NumDocResponsable = formFirma.NumDoc;
                firma.Parentesco = formFirma.Relacion;
            }

            //***
            Stream image = await sigPatient.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            if (image == null)
            {
                DependencyService.Get<IMessage>().ShortAlert(_languaje.Equals("en") ? "You must sign to continue" : "Debe firmar para continuar");
                
                return;
            }

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
            firma.Tipo = _languaje;
            firma.FecRegistro = DateTime.Now;

            /* llamar camara */
            await Navigation.PushModalAsync(new CameraPage(this), false);
            /***/
        }

        public async Task SendFirma()
        {
            await Send();
        }

        private async Task Send()
        {
            bar.IsRunning = true;
            btnAceptar.IsEnabled = false;

            var path = DependencyService.Get<IFileSystem>().GetExternalStorage();
            byte[] b = System.IO.File.ReadAllBytes(path + "/pic.jpg");
            String s = Convert.ToBase64String(b);
            firma.SingImage = s;
            firma.FecRegistroImage = DateTime.Now;


            var response = await _serviceFirma.SendHabeasData(firma, token);
            if (response.code == (int)HttpStatusCode.OK) await Navigation.PushModalAsync(new ConfirmacionHabeasData(this), false);
            else DependencyService.Get<IMessage>().ShortAlert(response.message);
            btnAceptar.IsEnabled = true;
            bar.IsRunning = false;
        }


        }    
}