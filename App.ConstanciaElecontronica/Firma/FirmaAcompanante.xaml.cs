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
    public partial class FirmaAcompanante : ContentView
    {
        public FirmaAcompanante()
        {
            InitializeComponent();
            lytOtro.IsVisible = false;
            lytHijo.IsVisible = false;
            
        }

        public void VerTelefono() {
            lytTelefono.IsVisible = true;
        }

        public string Nombre { get; set; }
        public int IdTipoDoc { get; set; }
        
        public string NumDoc { get; set; }
        public string Relacion { get; set; }
        public string Telefono { get; set; }


        public void Clear()
        {
            txtResponsable.Text="";
            txtNumDoc.Text = "";
            cbxTipoRelacion.SelectedIndex = -1;
            cbxTipoDoc.SelectedIndex = -1;
            txtOtra.Text = "";
            txtHijo.Text = "";
            txtTelefono.Text = "";
            
        }

            public string Validation () {
            string message = string.Empty;
            var languaje = CrossMultilingual.Current.CurrentCultureInfo;
           Nombre = txtResponsable.Text;
            if (string.IsNullOrEmpty(Nombre))
            {
                txtResponsable.Focus();
                return languaje.ToString()=="en" ? "You must complete the name of the companion" : "Debe completar el nombre del acompañante";          
            }
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
            int selectRelation = cbxTipoRelacion.SelectedIndex;
            if (selectRelation==-1)
            {
                cbxTipoRelacion.Focus();
                
                return languaje.ToString() == "en" ? "You must select a type of relationship with the patient" : "Debe seleccionar un tipo de relación con el paciente";

            }
            Relacion= (string)cbxTipoRelacion.SelectedItem;

            switch (selectRelation)
            {
                case 5:
                    Relacion = $"{txtOtra.Text}";
                    if (string.IsNullOrEmpty(Relacion)) {
                        txtOtra.Focus();
                        
                        return languaje.ToString() == "en" ? "You must specify what the relationship is" : "Debe especificar cuál es la relación";
                    }
                    
                    break;
                case 3:
                    if (string.IsNullOrEmpty(txtHijo.Text))
                    {
                        txtHijo.Focus();                        
                        return languaje.ToString() == "en" ? "You must specify No." : "Debe especificar No."; 
                    }
                    Relacion += $" No. {txtHijo.Text}";
                    break;

            }

            if (lytTelefono.IsVisible) { 
                Telefono = txtTelefono.Text;
                if (string.IsNullOrEmpty(Telefono))
                {
                    txtTelefono.Focus();
                    return "Debe completar el teléfono de contacto";          
                }
            }

            return message;
        }

        private void cbxTipoRelacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectRelation = cbxTipoRelacion.SelectedIndex;
            lytOtro.IsVisible = false;
            lytHijo.IsVisible = false;
            switch (selectRelation) { 
                case 5:
                    lytOtro.IsVisible = true;
                    txtOtra.Focus();
                    break;
                case 3:
                    lytHijo.IsVisible = true;
                    txtHijo.Focus();
                    break;                

            }
        }


        private void cbxTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNumDoc.Keyboard = (cbxTipoDoc.SelectedIndex == 2 || cbxTipoDoc.SelectedIndex == 5 || cbxTipoDoc.SelectedIndex == 4
                || cbxTipoDoc.SelectedIndex == 9 || cbxTipoDoc.SelectedIndex == 11) ? Keyboard.Default : Keyboard.Numeric;
        }
    }
}