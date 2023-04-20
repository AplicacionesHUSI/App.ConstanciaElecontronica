using App.ConstanciaElecontronica.Camera;
using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.Services;
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

namespace App.ConstanciaElecontronica.InstruccionesEgreso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstruEgresoFirma : ContentPage
    {

        private readonly ServicePatient _service;
        private readonly ServiceInstruccionesEgreso _serviceEgreso;
        private readonly ServiceConfiguracion _serviceConf;
        private bool firmaPaciente;
        private bool firmaAcompanante;
        private readonly string token;
        private FirmaPaciente _firma;
        private List<TipoDocumento> tiposDocumento; 
        private readonly InicioInstruccionesEgreso _form;

        public InstruEgresoFirma(ServicePatient service,
                               ServiceInstruccionesEgreso serviceInsEgreso,
                               ServiceConfiguracion serviceConf,
                               FirmaPaciente firma, InicioInstruccionesEgreso form =null)
        {
            InitializeComponent();
            this.token = App.Current.Properties["token"].ToString();
            this._service = service;
            this._serviceEgreso = serviceInsEgreso;
            this._serviceConf = serviceConf;
            this._firma = firma;

            this.cargarListas();
            this.cargarInstrucciones();
            _form = form;

        }

        private void cargarInstrucciones()
        {
            DateTime fecha = DateTime.Now;
            CultureInfo myCIintl = new CultureInfo("es-CO", false);

            lblError.IsVisible = false;
            var response = _service.GetPatient(this._firma.IdAtencion.ToString(), token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                var paciente = response.data;
                lblTipoDoc.Text = paciente.TipoDoc;
                lblIdentificacion.Text = paciente.NumDocumento;
                lblNombres.Text = paciente.NomCliente + " " + paciente.ApeCliente;
                lblFecha.Text = fecha.Date.ToString("d", myCIintl.Parent);
                lblAtencion.Text = _firma.IdAtencion.ToString();
                this.firmaPaciente = this._firma.IndMismoPaciente;
                this.firmaAcompanante = !this._firma.IndMismoPaciente;

                
                lblContenido.Text = "Yo <b>" + paciente.NomCliente + " " + paciente.ApeCliente + "</b> identificado con <b> " + paciente.TipoDoc + " " + paciente.NumDocumento + "</b>, doy constancia que he recibido y entendido las instrucciones de egreso, entregadas en la atención <b>"
                                    + this._firma.IdAtencion + "</b> del <b>" + fecha.Day + " de " + myCIintl.DateTimeFormat.GetMonthName(fecha.Month).ToString() +  " del " + fecha.Year + "</b>.";


                this.cargarFirma();

            }

            if (response.code == (int)HttpStatusCode.NotFound)
            {
                lblContenido.Text = "";
                lblError.Text = "Atención no encontrada";
                lblError.IsVisible = true;
            }

            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                lblContenido.Text = "";
                lblError.Text = "Atención no encontrada";
                lblError.IsVisible = true;
                App.Current.Logout();
            }

            if (response.code == (int)HttpStatusCode.InternalServerError)
            {
                lblContenido.Text = "";
                lblError.Text = "Error";
                lblError.IsVisible = true;
            }

            
            
        }


        private void cargarFirma()
        {
            lblError.IsVisible = false;
            
            if (this._firma.IdAtencion > 0)
            {

                this.firmaPaciente = _firma.IndMismoPaciente;
                this.firmaAcompanante = !_firma.IndMismoPaciente;

                var tipoDoc = "";
                if (_firma.IdTipoDocResponsable != null)
                    tipoDoc = this.tiposDocumento.Where(t => t.idTipoDoc == _firma.IdTipoDocResponsable).Select(t=> t.nomTipoDoc).FirstOrDefault();

                this.lblAcompanante.Text = _firma.NomResponsable +", "+ _firma.Parentesco +  " [ " + tipoDoc + " " + _firma.NumDocResponsable + " ]";

            }
            else
            {
                this.firmaPaciente = false;
                this.firmaAcompanante = false;

                this.lblAcompanante.Text = "";

                lblError.Text = "Error en Instrucciones de Egreso";
                lblError.IsVisible = true;
            }

            mostrarFirmasAsync();
        }


        private async Task mostrarFirmasAsync()
        {
            lblSignPaciente.IsVisible = this.firmaPaciente;
            lblPacienteFirma.IsVisible = this.firmaPaciente;
            signPaciente.IsVisible = this.firmaPaciente;

            lblSignAcompanante.IsVisible = this.firmaAcompanante;
            lblAcompanante.IsVisible = this.firmaAcompanante;
            signAcompanante.IsVisible = this.firmaAcompanante;

            await Task.Delay(1000);

            var lastChild = lyContenido.Children.LastOrDefault();
            if (lastChild != null)
                scrollInstrucciones.ScrollToAsync(lastChild, ScrollToPosition.End, true);


        }





        private async void Button_Clicked(object sender, EventArgs e)
        {
            lblError.IsVisible = false;

            try
            {
                DateTime fecha = DateTime.Now;
                CultureInfo myCIintl = new CultureInfo("es-CO", false);

                lblError.IsVisible = false;


                Stream image = null;


                //Firma de Paciente

                if (_firma.IndMismoPaciente)
                {

                    image = await signPaciente.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);

                    if (image == null)
                    {
                        lblError.Text = "Debe firmar para continuar";
                        lblError.IsVisible = true;
                        return;
                    }

                    //btnContinuar.IsEnabled = false;
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

                    _firma.Image = base64;

                }
                else
                {
                    image = await signAcompanante.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);

                    if (image == null)
                    {
                        lblError.Text = "Debe firmar para continuar";
                        lblError.IsVisible = true;
                        return;
                    }

                    //btnContinuar.IsEnabled = false;
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

                    _firma.Image = base64;
                }



                await Navigation.PushModalAsync(new CameraPage(this), false);


            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                lblError.IsVisible = true;
            }

        }

        public async Task SendFirma() {
            await Send();
        }



        private async Task Send() {


            bar.IsRunning = true;
            btnAceptar.IsEnabled = false;
            lblbar.IsVisible = true;

            var path = DependencyService.Get<IFileSystem>().GetExternalStorage();
            byte[] b = System.IO.File.ReadAllBytes(path + "/pic.jpg");
            String s = Convert.ToBase64String(b);
            _firma.SingImage = s;
            _firma.FecRegistro = DateTime.Now;

            _firma.UsrWindowsReg = App.Current.Properties["name"].ToString();
            _firma.tipo = "InstrucionesEgreso";
            RequestFirmaInstruEgreso request = new RequestFirmaInstruEgreso();
            request.Firma = _firma;
            var response = await _serviceEgreso.EnviarFirma(request, token);

            if (response.code == (int)HttpStatusCode.OK)
            {
                DependencyService.Get<IMessage>().ShortAlert("Documento Firmado");
                await Navigation.PushModalAsync(new ConfirmacionEgreso(this), false);
            }

            if (response.code == (int)HttpStatusCode.NotFound)
            {
                lblError.Text = "Acción inválida";
                lblError.IsVisible = true;
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                lblError.Text = "Acción no autorizada";
                lblError.IsVisible = true;
                App.Current.Logout();
            }

            if ((response.code == (int)HttpStatusCode.InternalServerError) || (response.code == 0))
            {
                lblError.Text = "No se ha guardado la firma";
                lblError.IsVisible = true;
            }


            bar.IsRunning = false;
            btnAceptar.IsEnabled = true;
            lblbar.IsVisible = false;

        }

        private void cancelarFirma(object sender, EventArgs e)
        {
            closeView(true);
        }

        public async void closeView(bool animated)
        {
            await Navigation.PopModalAsync(animated);

            if (animated)
            {
                try
                {
                    var response = _serviceEgreso.CancelarFirma(token);

                }
                catch (Exception e)
                {
                }
            }
            else {
                if (_form != null) _form.ClearForm();
            }

        }

        
        private void cargarListas()
        {
            this.tiposDocumento = _serviceConf.GetTiposDocumento(this.token).data;
        }
    }
}