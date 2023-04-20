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
    public partial class ConstanciaHabeasDataEvent : ContentPage
    {
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private bool isSamePatient;
        
        private readonly string token;
        private readonly string _languaje;
        private readonly HabeasDataEvent _data;
        private FirmaHabeasData firma ;
        public ConstanciaHabeasDataEvent(HabeasDataEvent data)
        {
            _data = data;
            this.isSamePatient = data.IndMismoPaciente;
            
            InitializeComponent();
            token = App.Current.Properties["token"].ToString();
           
           
            _languaje = data.Idioma;
            LoadDataPatiet(data.IdTipoDoc, data.NumDoc);
            
        }

        private async Task mostrarFirmasAsync()
        {
            try
            {


                await Task.Delay(1000);

                var lastChild = lyContenido.Children.LastOrDefault();
                if (lastChild != null)
                    await scrollCarta.ScrollToAsync(lastChild, ScrollToPosition.End, true);
            }
            catch (Exception e) {
                Console.WriteLine("error :: "+e.Message);
            }


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

                if (isSamePatient)
                {
                    gridPaciente.IsVisible = true;

                    var msg = _languaje.Equals("en") ? "Patient:" : "paciente:";
                    lblSignPaciente.Text = msg;                    
                    this.lblPacienteFirma.Text =  " [ " + patient.NomCliente + " " + patient.ApeCliente + " ]";
                }
                else {
                    gridPaciente.IsVisible = true;
                    var msg = _languaje.Equals("en") ? "Companion:" : "Acompañante:";
                    lblSignPaciente.Text = msg;


                    var serviceConf=new ServiceConfiguracion (Secrets.APIConstancia);

                              var tiposDocumento = serviceConf.GetTiposDocumento(this.token).data;
                            var tipoDocUno = tiposDocumento.Where(t => t.idTipoDoc == _data.IdTipoDocResponsable).Select(t => t.nomTipoDoc).FirstOrDefault();
                            this.lblPacienteFirma.Text =  _data.Responsable +  " [ " + tipoDocUno +" "+ _data.NumDocResponsable +  " ]";

                }
                mostrarFirmasAsync();
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
            /*if (!animated) {
                _inicio.ClearForm();
                _inicio.ClearFormHead();
            }*/
            
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
               
                firma.Responsable = _data.Responsable;
                firma.IdTipoDocResponsable = (byte)_data.IdTipoDocResponsable;
                firma.NumDocResponsable = _data.NumDocResponsable;
                firma.Parentesco = _data.Parentesco;
            }

            //***
            Stream image = await sigPatient.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            if (image == null)
            {
                var msg = _languaje.Equals("en") ? "You must sign to continue" : "Debe firmar para continuar";
                lblError.IsVisible = true;
                lblError.Text = msg;
                
                DependencyService.Get<IMessage>().ShortAlert(msg);
                

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
            firma.Tipo = _languaje;
            //**

            /* llamar camara */
            await Navigation.PushModalAsync(new CameraPage(this), false);
            /***/

        }

        public async Task SendFirma() {
            await Send();   
        }

        private async Task Send()
        {


            var path = DependencyService.Get<IFileSystem>().GetExternalStorage();
            byte[] b = System.IO.File.ReadAllBytes(path + "/pic.jpg");
            String s = Convert.ToBase64String(b);
            firma.SingImage = s;
            firma.FecRegistroImage = DateTime.Now;


            var response = await _serviceFirma.SendHabeasData(firma, token);
            if (response.code == (int)HttpStatusCode.OK) await Navigation.PushModalAsync(new ConfirmacionHabeasDataEvent(this), false);
            else DependencyService.Get<IMessage>().ShortAlert(response.message);
            btnAceptar.IsEnabled = true;
            bar.IsRunning = false;

        }


    }
}