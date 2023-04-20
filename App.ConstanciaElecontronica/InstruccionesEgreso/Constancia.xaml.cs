using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Constancia : ContentPage
    {
        private List<Instruccion> instrucciones;
        private string IdAtencion;
        private readonly string token;
        private bool isSamePatient;
        Inicio _page;

        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private readonly ServiceInstruction _serviceInstr = new ServiceInstruction(Secrets.APIConstancia);
        public Constancia(string atencion, List<Instruccion> listInstrucciones, bool isSamePatient, Inicio page)
        {
            InitializeComponent();
            token = App.Current.Properties["token"].ToString();
            instrucciones = listInstrucciones;
            IdAtencion = atencion;
            this.isSamePatient = isSamePatient;
            setInstruccions();
            hideResponsable();
            loadPatient();
            _page = page;
        }

        private void loadPatient()
        {
            btnGuardar.IsEnabled = true; 
            var response = _service.GetPatient(IdAtencion,token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                lblAtencion.Text = IdAtencion;
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
                lblPatient.Text = "No se encuentra paciente ";
                lblPatient.TextColor = Color.DarkRed;
               
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
        }

        private void hideResponsable()
        {
            if (isSamePatient) {
                lytResponsable.IsVisible = false;
                bxvResponsable.IsVisible = false;
                bxvResponsable1.IsVisible = false;
            }
        }

        private void setInstruccions()
        {
            foreach (var i in instrucciones)
            {
                lytInstrucciones.Children.Add(new Label()
                {
                    Text = "  \u2022 " + i.Informe,
                    VerticalOptions = LayoutOptions.Center
                });
            }
        }

        private  void Button_Clicked(object sender, EventArgs e)
        {
           
            closeView(true);
        }

        public async void closeView( bool animated) {
            await Navigation.PopModalAsync(animated);
            if (!animated) _page.ClearForm();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            
            var firma = new FirmaEgreso()
            {
                Atencion = IdAtencion,
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
            if (image == null) {
                DependencyService.Get<IMessage>().ShortAlert("Debe firmar para continuar");
                return;
            }
            btnGuardar.IsEnabled = false;
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
            //**
            var request = new RequestFirmaEgreso() { 
            Firma=firma,
            Instrucciones=instrucciones
            };


            var response = _serviceInstr.SendInstruction(request,token);
            if (response.code == (int)HttpStatusCode.OK) await Navigation.PushModalAsync(new Confirmacion(this), false);
            else DependencyService.Get<IMessage>().ShortAlert(response.message);
            btnGuardar.IsEnabled = true;
        }

        private async  Task<string> getBase64()
        {
            Stream image = await sigPatient.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
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
            return base64;
        }       
    }
}