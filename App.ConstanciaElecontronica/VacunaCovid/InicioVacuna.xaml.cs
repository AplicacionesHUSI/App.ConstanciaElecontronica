using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Firma;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.VacunaCovid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioVacuna : ContentPage
    {
        private readonly ServiceCuidadosEnfermeria _serviceFirma = new ServiceCuidadosEnfermeria(Secrets.APIConstancia);
        private readonly ServiceVacunacion _serviceVacunas = new ServiceVacunacion(Secrets.APIConstancia);
        private readonly ServicePatient _service;        
        private string Atencion = string.Empty;
        private bool Encontrado = false;
        private bool Aceptado = false;
        private readonly string token;
        private List<Vacuna> vacunas;

        public InicioVacuna(ServicePatient service)
        {
            InitializeComponent();
            _service = service;
            token = App.Current.Properties["token"].ToString();
          //  user.Text = "Inicio (" + App.Current.Properties["name"] + ")";
            ClearForm();

            loadVacunas();
           
        }

        private void loadVacunas()
        {
            var response = _serviceVacunas.GetVacunas(token);
            
            if(response.code == (int)HttpStatusCode.OK)
            {
                vacunas = response.data;
                var vacunasList = new List<string>();
                foreach (var v in vacunas) vacunasList.Add(v.Nombre);
                cbxVacuna.ItemsSource = vacunasList;
            }

            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
        }

        public void ClearForm()
        {
            txtValidation.IsVisible = false;
            Encontrado = false;
            Aceptado = false;

            lytButtons.IsVisible = false;
            lytPatient.IsVisible = false;
         
            boxExternal1.IsVisible = false;
            boxInternal1.IsVisible = false;
            
            Atencion = null;
            lytRazon.IsVisible = false;
            txtRazon.Text = "";
            //txtAtencion.Text = "";
        }

        void BtnLogoutClick(object sender, EventArgs e)
        {
            App.Current.Logout();
        }

        private void BtnLimpiarClicked(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnAtencion(object sender, EventArgs e)
        {
            var doc = txtDocumento.Text;

            if (string.IsNullOrEmpty(doc))
            {
                DependencyService.Get<IMessage>().ShortAlert("El documento no puede ser vacio");
                return;
            }
            int selectedIndex = cbxTipoDoc.SelectedIndex;
            if (selectedIndex == -1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe elegir un tipo de documento");
                return;
            }


            var tipoDoc = (string)cbxTipoDoc.ItemsSource[selectedIndex];
            var IdTipoDoc = Util.Util.GetIdDoc(tipoDoc);
            //TipoDoc = IdTipoDoc;
            //Documento = doc;
            loadPatietn(IdTipoDoc, doc);
           
            

        }

        private void loadPatietn(int IdtipoDoc, string documento)
        {
            txtValidation.IsVisible = false;
            Aceptado = false;
            Encontrado = false;
            Atencion = null;
            var response = _service.GetPatient(IdtipoDoc, documento, token);
            showLayouts();
            if (response.code == (int)HttpStatusCode.OK)
            {
                var patient = response.data;
                lblPatient.Text = patient.NomCliente + " " + patient.ApeCliente;
                lblPatient.TextColor = Color.Default;
                lblDocumento.Text = patient.NumDocumento;
                lblTipoDoc.Text = patient.TipoDoc;
                lblSexo.Text = patient.IdSexo == 1 ? "Masculino" : "Femenino";
                var currentDate = DateTime.Now;
                var age = currentDate.Year - patient.FecNacimiento.Year;
                // Go back to the year the person was born in case of a leap year
                if (patient.FecNacimiento.Date > currentDate.AddYears(-age)) age--;
                lblEdad.Text = " " + age + " años";
                Encontrado = true;
                //validateSign();

                var atenciones = patient.Atenciones.Where(x => x.IdAtencionTipo == 61).ToList();//**** tipo base in LIST ???
                if (atenciones.Count > 0)
                {
                    lblValidationAtencion.IsVisible = false;
                    cbxAtenciones.IsVisible = true;
                    var atenList = new List<string>();
                    foreach (var x in atenciones) {
                        atenList.Add(x.IdAtencion+"");                        
                    }
                    cbxAtenciones.ItemsSource = atenList;
                    cbxAtenciones.SelectedIndex = 0;
                    
                }
                else {
                    lblValidationAtencion.IsVisible = true;
                    cbxAtenciones.IsVisible = false;
                }
               
            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {               
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                lblDocumento.Text = "";
                lblTipoDoc.Text = "";
                lblSexo.Text = "";
                lblEdad.Text = "";
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
            if (response.code == (int)HttpStatusCode.InternalServerError)
            {
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                lblDocumento.Text = "";
                lblTipoDoc.Text = "";
                lblSexo.Text = "";
                lblEdad.Text = "";
                hideLayouts();
                DependencyService.Get<IMessage>().ShortAlert(
                                ((response.message!=null) && (response.message.Length > 0)) ? 
                                  response.message : "Paciente no encontrado");
            }
        }

        private void showLayouts()
        {
            lytButtons.IsVisible = true;
            lytPatient.IsVisible = true;
          
            boxExternal1.IsVisible = true;
            boxInternal1.IsVisible = true;
       
        }

        private void hideLayouts()
        {
            lytButtons.IsVisible = false;
            //lytPatient.IsVisible = false;

            boxExternal1.IsVisible = false;
            boxInternal1.IsVisible = false;

        }


        private async void BtnContinuar(object sender, EventArgs e)
        {
            var i = cbxAtenciones.SelectedIndex;
            if (i > -1) Atencion = (string)cbxAtenciones.ItemsSource[i];

            if (!Encontrado || string.IsNullOrEmpty(Atencion)) {
                DependencyService.Get<IMessage>().ShortAlert("Debe buscar un paciente con atención activa.");
                return;
            }
            var pos = cbxVacuna.SelectedIndex;
            if (pos==-1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar una vacuna.");
                return;
            }


            if (Aceptado)
            {
                DependencyService.Get<IMessage>().ShortAlert("Ya se acepto previamente el cosentimiento.");
                //return;
            }

            var isPatienet = cbxPaciente.IsChecked;
            /*if (!isPatienet && !cbxResponsable.IsChecked)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe seleccionar la persona que firma (paciente/responsable)");
                return;
            }*/
            var razon = "";
            if (cbxResponsable.IsChecked) {
                razon = txtRazon.Text;
            }
            await Navigation.PushModalAsync(new ConsentimientoInformado(this,Atencion, isPatienet,vacunas[cbxVacuna.SelectedIndex],cbxUnaDosis.IsChecked, cbxResponsableMenor.IsChecked,razon));
        }


        private void cbxResponsable_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            if (cbxResponsable.IsChecked)
            { 
                cbxPaciente.IsChecked = false;
                cbxResponsableMenor.IsChecked = false;
                lytRazon.IsVisible = true;
                txtRazon.Focus();
            }
            else lytRazon.IsVisible = false;
        }

        private void cbxPaciente_CheckedChanged_1(object sender, CheckedChangedEventArgs e)
        {
            if (cbxPaciente.IsChecked) 
            {
                cbxResponsable.IsChecked = false;
                cbxResponsableMenor.IsChecked = false;
            }
        }

        private void cbxResponsableMenor_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (cbxResponsableMenor.IsChecked)
            {
                cbxPaciente.IsChecked = false;
                cbxResponsable.IsChecked = false;
            }
        }
        private void validateSign()
        {
            var response = _serviceFirma.GetFirma(Atencion, "VacunacionCovid19", token);
            if (response.code == (int)HttpStatusCode.OK)
            {
                Aceptado = true;
                txtValidation.IsVisible = true;
            }
        }

        private void cbxDosDosis_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxUnaDosis.IsChecked = !cbxDosDosis.IsChecked;
        }

        private void cbxUnaDosis_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            cbxDosDosis.IsChecked = !cbxUnaDosis.IsChecked;
        }

     
    }
}