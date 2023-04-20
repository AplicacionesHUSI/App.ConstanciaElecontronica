using Plugin.Multilingual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Firma
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirmaAcompananteVacunacion : ContentView
    {
        public FirmaAcompananteVacunacion()
        {
            InitializeComponent();
          
        }

        public string Nombre { get; set; }
        public int IdTipoDoc { get; set; }
        
        public string NumDoc { get; set; }
        public string Relacion { get; set; }


        public string Validation () {
            string message = string.Empty;
            var languaje = CrossMultilingual.Current.CurrentCultureInfo;
           Nombre = txtResponsable.Text;
            if (string.IsNullOrEmpty(Nombre))
            {
                txtResponsable.Focus();
                return languaje.ToString()=="en" ? "You must complete the name of the companion" : "Debe completar los nombres del acompañante";          
            }
            var apellidos=txtResponsableApellidos.Text;
            if (string.IsNullOrEmpty(apellidos))
            {
                txtResponsable.Focus();
                return languaje.ToString() == "en" ? "You must complete the name of the companion" : "Debe completar los apellidos del acompañante";
            }
            Nombre += ";" + apellidos;

            int selectedIndex = cbxTipoDoc.SelectedIndex;
            if (selectedIndex == -1)
            {
                cbxTipoDoc.Focus();
                return languaje.ToString() == "en" ? "You must select a type of companion document" : "Debe seleccionar un tipo de documento del acompañante";
                
            }
            var tipoDoc = (string)cbxTipoDoc.SelectedItem;
            IdTipoDoc = Util.Util.GetIdDoc(tipoDoc);


            NumDoc = txtNumDoc.Text;
            if (string.IsNullOrEmpty(NumDoc))
            {
                txtNumDoc.Focus();

                return languaje.ToString() == "en" ? "You must complete the document number of the companion" : "Debe completar el número de documento del acompañante";

            }

            //Relacion = txtParentesco.Text;

            Relacion = txtLugar.Text;
            if (string.IsNullOrEmpty(Relacion))
            {
                txtLugar.Focus();

                return  "Debe completar el lugar de expedición del documento";

            }


            return message;
        }

        
     

        private void cbxTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNumDoc.Keyboard = cbxTipoDoc.SelectedIndex == 2 || cbxTipoDoc.SelectedIndex == 4 ? Keyboard.Default : Keyboard.Numeric;
        }
    }
}