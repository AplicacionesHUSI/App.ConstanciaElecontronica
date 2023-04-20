using App.ConstanciaElecontronica.CartaCompromisoCovid;
using App.ConstanciaElecontronica.CuidadosEnfermeria;
using App.ConstanciaElecontronica.EntregaInformacion;
using App.ConstanciaElecontronica.Firma;
using App.ConstanciaElecontronica.HabeasData;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.InstruccionesEgreso;
using App.ConstanciaElecontronica.Inyectologia;
using App.ConstanciaElecontronica.Services;
using App.ConstanciaElecontronica.SignalR;
using App.ConstanciaElecontronica.TratamientoFarmacologico;
using App.ConstanciaElecontronica.VacunaCovid;

using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Frame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1 : MasterDetailPage
    {
        public MasterDetailPage1()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            viewInit();


        }

        private void viewInit()
        {
            var Roles = (List<string>)App.Current.Properties["roles"];
            if (Roles.Count > 1)
            {
                var page = new SelectRole(this, Roles);
                page.Title = "Inicio";
                Detail = new NavigationPage(page);
            }
            else SetPage(App.Current.Properties["role"].ToString());
        }

        private void Logout()
        {
            App.Current.Logout();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailPage1MasterMenuItem;
            if (item == null)
                return;

            if (item.Id == "Logout") { Logout();
                return;
            }
            App.Current.Properties["role"] = item.Id;
            var page = GetPage(item.Id);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private readonly ServiceCartaCompromiso _serviceCarta = new ServiceCartaCompromiso(Secrets.APIConstancia);
        private readonly ServiceInstruccionesEgreso _serviceInsEgreso = new ServiceInstruccionesEgreso(Secrets.APIConstancia);
        private readonly ServiceConfiguracion _serviceConf = new ServiceConfiguracion(Secrets.APIConstancia);


        private Page GetPage(string role)
        {
            Page inicio;
            
            switch (role)
            {
                case "Init":
                    var Roles = (List<string>)App.Current.Properties["roles"];                    
                        inicio = new SelectRole(this,Roles);
                    break;
                case "HabeasData":
                    inicio = new InicioHabeasData(_service);
                    break;
                case "Instrucciones":
                    inicio = new Inicio(_service);
                    break;
                case "CuidadosEnfermeria":
                       if (!App.Current.Properties.ContainsKey("job"))
                           inicio = new SolitudFirma(this);
                     else 
                    inicio = new InicioCuidadosEnfermeria(_service);
                    break;
                case "TratamientoFarmacologico":
                    inicio = new InicioTratamientoFarmacologico(_service);
                    break;
                case "Inyectologia":
                     if (!App.Current.Properties.ContainsKey("job"))
                        inicio = new SolitudFirma(this);
                       else 
                    inicio = new InicioInyectologia(_service);
                    break;
                case "EntregaInformacion":
                    inicio = new InicioEntregaInformacion(_service);
                    break;
                case "VacunaCovid":
                    if (!App.Current.Properties.ContainsKey("lugarExp"))
                        inicio = new SolitudFirmaVacunacion(this);
                    else
                        inicio = new InicioVacuna(_service);
                    break;

                case "CartaCompromisoCovid":
                    inicio = new InicioCartaCompromiso(_service);
                    break;
                
                case "InstruccionesEgreso":    
                    inicio = new InicioInstruccionesEgreso(this._service);
                    break;
                default:
                    inicio = new Inicio(_service);
                    break;
            }
            return inicio;
        }

        public void SetPage(string role) {
            var page = GetPage(role);
            page.Title = Util.Util.GetNameMenu(role);
            Detail = new NavigationPage(page);
        }
    }
}