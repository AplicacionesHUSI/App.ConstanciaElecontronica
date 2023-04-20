using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Frame;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Process;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Firma
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SolitudFirmaVacunacion : ContentPage
    {
        MasterDetailPage1 _service;
        private readonly ServicePersonal _servicePersonal = new ServicePersonal(Secrets.APIConstancia);
        public SolitudFirmaVacunacion(MasterDetailPage1 ilm)
        {
            InitializeComponent();
            _service = ilm;
            txtPerson.Text= "Bienvenido "+App.Current.Properties["userName"].ToString() +",";
            var token = App.Current.Properties["token"].ToString();
            
            var response  = _servicePersonal.GetPersonal(App.Current.Properties["name"].ToString(),token);
            if (response.code == 200) {
                var persona = response.data;
                txtDocumento.Text =  persona.NumDocumento;
            }
            
            
        }
        private async void btnContinuar_Clicked(object sender, EventArgs e)
        {
            var ciudad = txtCiudad.Text;
            if (string.IsNullOrEmpty(ciudad) )
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe digitar un lugar de expedición");
                return;
            }
            

            var image = await sigPerson.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            if (image == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe firmar para continuar");
                return;
            }
            btnContinuar.IsEnabled = false;
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
            
            App.Current.Properties["sign"] = base64;
            App.Current.Properties["lugarExp"] = ciudad;
            btnContinuar.IsEnabled = true;  
            var role=App.Current.Properties["role"].ToString();
            _service.SetPage(role);

        }
    }
}