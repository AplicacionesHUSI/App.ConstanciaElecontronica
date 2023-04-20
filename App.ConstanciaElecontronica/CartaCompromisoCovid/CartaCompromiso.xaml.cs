using App.ConstanciaElecontronica.Camera;
using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Services;
using App.ConstanciaElecontronica.VacunaCovid;
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

namespace App.ConstanciaElecontronica.CartaCompromisoCovid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartaCompromiso : ContentPage
    {

        private readonly ServicePatient _service;
        private readonly ServiceCartaCompromiso _serviceCarta;
        private readonly ServiceConfiguracion _serviceConf;
        private bool firmaPaciente;
        private bool firmaAcompaUno;
        private bool firmaAcompaDos;
        private readonly string token;
        private CartaCompromisoCovid19 _carta;
        private List<TipoDocumento> tiposDocumento;
        private readonly InicioCartaCompromiso _inicioCarta;
        public CartaCompromiso(ServicePatient service,
                               ServiceCartaCompromiso serviceCarta,
                               ServiceConfiguracion serviceConf,
                               CartaCompromisoCovid19 carta,
                               InicioCartaCompromiso inicioCarta=null)
        {
            InitializeComponent();
            this.token = App.Current.Properties["token"].ToString();
            this._service = service;
            this._serviceCarta = serviceCarta;
            this._serviceConf = serviceConf;
            this._carta = carta;
            _inicioCarta = inicioCarta;
            this.cargarListas();
            this.cargarCartaCompromiso();

        }
        
        private void cargarCartaCompromiso()
        {
            DateTime fecha = DateTime.Now;
            CultureInfo myCIintl = new CultureInfo("es-CO", false);

            lblError.IsVisible = false;
            var response = _service.GetPatient(this._carta.IdAtencion.ToString(),token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                var paciente = response.data;
                _carta.IdCliente = paciente.IdCliente;
                lblTipoDoc.Text = paciente.TipoDoc;
                lblIdentificacion.Text = paciente.NumDocumento;
                lblNombres.Text = paciente.NomCliente + " " + paciente.ApeCliente;
                lblFecha.Text = fecha.Date.ToString("d", myCIintl.Parent);
                lblAtencion.Text = this._carta.IdAtencion.ToString();
                this.firmaPaciente = true;
                this.firmaAcompaUno = false;
                this.firmaAcompaDos = false;

                this.lblPacienteFirma.Text = paciente.NomCliente + " " + paciente.ApeCliente;
                

                this.cargarFirma();

            }

            if (response.code == (int)HttpStatusCode.NotFound)
            {
                lblError.Text = "Atención no encontrada";
                lblError.IsVisible = true;
            }
            
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                lblError.Text = "Atención no encontrada";
                lblError.IsVisible = true;
                App.Current.Logout();
            }
            
            if (response.code == (int)HttpStatusCode.InternalServerError)
            {
                lblError.Text = "Error";
                lblError.IsVisible = true;
            }


            lblDiaDeFirma.Text = "Firman en <b>Bogotá</b> a los <b>" + fecha.Day + "</b> días, del mes de <b>" + myCIintl.DateTimeFormat.GetMonthName(fecha.Month).ToString() + "</b> del año <b>" + fecha.Year + "</b>:";

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

                    if (_carta.IndMismoPaciente) { 

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

                        _carta.ImageUno = base64;

                    }


                //Firma de Acompañante Uno

                if (_carta.IndAcompananteUno)
                {

                    image = await signAcompananteUno.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);

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

                    _carta.ImageUno = base64;

                }

                
                //Firma de Acompañante Dos

                if (_carta.IndAcompananteDos)
                {

                    image = await signAcompananteDos.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);

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

                    _carta.ImageDos = base64;

                }

                _carta.UsrWindowsReg = App.Current.Properties["name"].ToString();
                _carta.TipoFirma = "CartaCompromisoCovid";


                /* llamar camara */
                await Navigation.PushModalAsync(new CameraPage(this), false);
                /***/
                
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
                lblError.IsVisible = true;
            }

        }

        public async Task enviarFirma() {
            await SendFirma();
        }
        private async Task SendFirma() {

            bar.IsRunning = true;
            btnAceptar.IsEnabled = false;
            lblbar.IsVisible = true;
            
            var path = DependencyService.Get<IFileSystem>().GetExternalStorage();
            byte[] b = System.IO.File.ReadAllBytes(path + "/pic.jpg");
            String s = Convert.ToBase64String(b);
            _carta.SingImage = s;
            _carta.FecRegistro = DateTime.Now;

            var response = await _serviceCarta.EnviarFirma(_carta, token);

            if (response.code == (int)HttpStatusCode.OK)
            {
                DependencyService.Get<IMessage>().ShortAlert("Documento Firmado");
                await Navigation.PushModalAsync(new ConfirmacionCartaComproCovid(this), false);
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

        
        private void cargarFirma()
        {
            lblError.IsVisible = false;
            //var response = _serviceCarta.GetCartaCompromisoCovid(this._carta.IdAtencion, token);
            if (this._carta.IdCliente > 0)
            {

                this.firmaPaciente = _carta.IndMismoPaciente;
                this.firmaAcompaUno = _carta.IndAcompananteUno;
                this.firmaAcompaDos = _carta.IndAcompananteDos;
                var tipoDocUno = "";
                var tipoDocDos = "";
                if(_carta.IdTipoDocResponsableUno != null)
                    tipoDocUno = this.tiposDocumento.Where(t => t.idTipoDoc == _carta.IdTipoDocResponsableUno).Select(t => t.nomTipoDoc).FirstOrDefault();
                
                if (_carta.IdTipoDocResponsableDos != null) 
                    tipoDocDos = this.tiposDocumento.Where(t => t.idTipoDoc == _carta.IdTipoDocResponsableDos).Select(t => t.nomTipoDoc).FirstOrDefault();

                this.lblAcompananteUno.Text = _carta.ParentescoUno + " [ " + tipoDocUno + " " + _carta.NumDocResponsableUno + " ]" ;
                this.lblAcompananteDos.Text = _carta.ParentescoDos + " [ " + tipoDocDos + " " + _carta.NumDocResponsableDos + " ]";

            }
            else
            {
                this.firmaPaciente = false;
                this.firmaAcompaUno = false;
                this.firmaAcompaDos = false;

                //this.lblPacienteFirma.Text = "";
                this.lblAcompananteUno.Text = "";
                this.lblAcompananteDos.Text = "";

                lblError.Text = "Error en la carga de la Carta";
                lblError.IsVisible = true;
            }

            mostrarFirmasAsync();
        }

        private async Task mostrarFirmasAsync()
        {
            lblSignPaciente.IsVisible = this.firmaPaciente;
            lblPacienteFirma.IsVisible = this.firmaPaciente;
            signPaciente.IsVisible = this.firmaPaciente;

            lblSignAcompaUno.IsVisible = this.firmaAcompaUno;
            lblAcompananteUno.IsVisible = this.firmaAcompaUno;
            signAcompananteUno.IsVisible = this.firmaAcompaUno;

            lblSignAcompaDos.IsVisible = this.firmaAcompaDos;
            lblAcompananteDos.IsVisible = this.firmaAcompaDos;
            signAcompananteDos.IsVisible = this.firmaAcompaDos;

            await Task.Delay(1000);

            var lastChild = lyContenido.Children.LastOrDefault();
            if (lastChild != null)
                scrollCarta.ScrollToAsync(lastChild, ScrollToPosition.End, true);


        }

        private void cargarListas()
        {
            this.tiposDocumento = _serviceConf.GetTiposDocumento(this.token).data;
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
                    var response = _serviceCarta.CancelarFirma(token);

                }
                catch (Exception e)
                {
                }
            }
            else
            {
                if (_inicioCarta != null) _inicioCarta.ClearForm();
            }


        }

    }
}