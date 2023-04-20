using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Services;
using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Inicio : ContentPage
    {
        private readonly ServicePatient _service;
        private readonly string token;
        private CheckBox[] listCbx;
        private List<Instruccion> instruccions;

        public Inicio(ServicePatient service)
        {
            InitializeComponent();
            token = App.Current.Properties["token"].ToString();
            _service = service;
            listCbx = new CheckBox[0];
            //  user.Text = "Inicio (" + App.Current.Properties["name"] + ")";
            ClearForm();
        }


        void BtnLogoutClick(object sender, EventArgs e)
        {
            App.Current.Logout();
        }
        private string IdAtencion=string.Empty;
        void BtnAtencionClick(object sender, EventArgs e)
        {
            loadData(atencion.Text);
        }

        public void loadData(string IdAtencion)
        {
            atencion.Text = IdAtencion;

            var response = _service.GetPatient(IdAtencion,token);
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
                lblEdad.Text=" "+age+" años";
                var responseInstrucciones = _service.GetInstrucciones(IdAtencion,token);
                if (responseInstrucciones.code == (int)HttpStatusCode.OK)
                {
                    instruccions = responseInstrucciones.data;
                    lblInstrucciones.IsVisible = true;
                    setInstrucciones(instruccions);
                } else 
                if (responseInstrucciones.code == (int)HttpStatusCode.NotFound) ShowValidationInstructions();


                if (responseInstrucciones.code == (int)HttpStatusCode.Forbidden)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Acceso denegado");
                }

            }
            if (response.code == (int)HttpStatusCode.NotFound)
            {
                ClearForm();
                lblPatient.Text = "Paciente no encontrado ";
                lblPatient.TextColor = Color.DarkRed;
                lytIntrucciones.Children.Clear();
                lytAll.IsVisible = false;
            }
            if (response.code == (int)HttpStatusCode.Unauthorized)
            {
                App.Current.Logout();
            }
        }

        public void ClearForm() {

            lytIntrucciones.Children.Clear();
            lytAll.IsVisible = false;
            lblInstrucciones.IsVisible = false;
            selectAll.IsChecked = false;
            atencion.Text = "";
            lblPatient.Text = "";
            
            lblDocumento.Text = "";
            lblTipoDoc.Text =  "";
            lblSexo.Text = "";
            lblEdad.Text = "";
            cbxPaciente.IsChecked = true;
            instruccions=new List<Instruccion>();
        }
        private void validation(object sender, EventArgs e) {
            foreach (var cbx in listCbx)
                if (!cbx.IsChecked) {
                    selectAll.CheckedChanged -= BtnAllClick;
                    selectAll.IsChecked = false;
                    selectAll.CheckedChanged += BtnAllClick;
                    return;
                }
            selectAll.CheckedChanged -= BtnAllClick;
            selectAll.IsChecked = true;
            selectAll.CheckedChanged += BtnAllClick;

        }
        private void setInstrucciones(List<Instruccion> instrucciones)
        {
            lytIntrucciones.Children.Clear();
            var ins = new List<Instruccion>();
            var n = instrucciones.Count;
            var j = 0;
            for (var i = 0; i < n; i++)
                if (instrucciones[i].IdFirma == null)
                { j++; ins.Add(instrucciones[i]);
                }
            
           lytAll.IsVisible = true; 
            listCbx = new CheckBox[j];
            if (j == 0) {
                ShowValidationInstructions();
            }
            j = 0;
            for (var i = 0; i < n; i++)
            {

                CheckBox cbx = new CheckBox()
                {
                    Color = Color.FromHex("005eff"),
                    VerticalOptions = LayoutOptions.Center
                   
                };
                if (instrucciones[i].IdFirma != null)
                {
                    cbx.IsEnabled = false;
                    cbx.IsChecked = true;
                    cbx.Color = Color.FromHex("afb7c4");
                }
                else
                {
                    cbx.CheckedChanged += validation;
                    listCbx[j] = cbx;
                    j++;
                }

                    var layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
                    Padding = new Thickness(20,0)
                 };

                layout.Children.Add(cbx);
                

                layout.Children.Add(new Label()
                {
                    Text = " " + instrucciones[i].Informe,
                    VerticalOptions = LayoutOptions.Center
                });

                lytIntrucciones.Children.Add(layout);

            }
            instruccions = ins;

        }

        private void ShowValidationInstructions()
        {
          //  lytIntrucciones.Children.Clear();
            lytIntrucciones.Children.Add(new Label()
            {
                Text = "No se encontraron instrucciones para aceptar.",
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.DarkRed

            });
            lytAll.IsVisible = false;
        }

        async void BtnContinuarClick(object sender, EventArgs e)
        {

            if (listCbx.Length == 0)
            {
                DependencyService.Get<IMessage>().ShortAlert("No se puede continuar porque no tiene instrucciones asignadas.");
                return;
            }

            var SelectedIntrucctions = new List<Instruccion>();
            var i = 0;
            foreach (var cbx in listCbx) {
                if (cbx.IsChecked) {
                    SelectedIntrucctions.Add(instruccions[i]);
                }
                i++;
            }
            if (SelectedIntrucctions.Count == 0)
            {
                DependencyService.Get<IMessage>().ShortAlert("Debe selecciona al menos una instrucción.");
                return;
            }

            var isSamePatient = cbxPaciente.IsChecked;
            await Navigation.PushModalAsync(new Constancia(atencion.Text,SelectedIntrucctions, isSamePatient,this));

        }
        void BtnAllClick(object sender, EventArgs e)
        {
            foreach (var cbx in listCbx)
            {
                cbx.CheckedChanged -= validation;
                cbx.IsChecked = selectAll.IsChecked;
                cbx.CheckedChanged += validation;

            }
        }
        

    }
}