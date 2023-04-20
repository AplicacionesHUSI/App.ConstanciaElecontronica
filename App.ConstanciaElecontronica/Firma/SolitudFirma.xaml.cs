using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Frame;
using App.ConstanciaElecontronica.Process;
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
    public partial class SolitudFirma : ContentPage
    {
        MasterDetailPage1 _service;
        public SolitudFirma(MasterDetailPage1 ilm)
        {
            InitializeComponent();
            _service = ilm;
            txtPerson.Text= "Bienvenido "+App.Current.Properties["userName"].ToString() +",";
        }
        private async void btnContinuar_Clicked(object sender, EventArgs e)
        {
            int selectedIndex = cbxCargo.SelectedIndex;
            if (selectedIndex == -1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar un tipo de documento");
                return;
            }
            var cargo = (string)cbxCargo.ItemsSource[selectedIndex];

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
            App.Current.Properties["job"] = cargo;
            btnContinuar.IsEnabled = true;
            var role=App.Current.Properties["role"].ToString();
            _service.SetPage(role);

        }
    }
}