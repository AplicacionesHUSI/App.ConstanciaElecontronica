using App.ConstanciaElecontronica.CartaCompromisoCovid;
using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.HabeasData;
using App.ConstanciaElecontronica.InstruccionesEgreso;
using App.ConstanciaElecontronica.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Plugin.Multilingual;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.SignalR
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InicioSolicitud : ContentPage
    {
        private readonly ServicePatient _service;
        private readonly ServiceCartaCompromiso _serviceCarta; 
        private readonly ServiceInstruccionesEgreso _serviceInsEgreso; 
        private readonly ServiceConfiguracion _serviceConf;
        
        private readonly string _APIEventos;
        private string ipAddress;
        
        private bool hubConectado = false;

        HubConnection hubConexion;

        public InicioSolicitud(ServicePatient servicePaciente,
                               ServiceCartaCompromiso serviceCarta,
                               ServiceInstruccionesEgreso serviceInsEgreso,
                               ServiceConfiguracion serviceConf,
                               string APIEventos)
        {
            InitializeComponent();

            this._APIEventos = APIEventos;
            this._service = servicePaciente;
            this._serviceCarta = serviceCarta;
            this._serviceInsEgreso = serviceInsEgreso;
            this._serviceConf = serviceConf;


            //catchIP();
            ConfigurateHub();
            ConnectAsync();
        }

        private void catchIP()
        {
            var myIpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            if (myIpAddress != null)
            {
                ipAddress = myIpAddress.ToString();
            }            
        }

        private void ConfigurateHub()
        {
            string url = this._APIEventos + "/eventHub";

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };


            /*, conf =>
            {
               conf.HttpMessageHandlerFactory = (x) => clientHandler;
            }*/
            hubConexion = new HubConnectionBuilder().WithUrl(url
            ).WithAutomaticReconnect().Build();

            hubConexion.On("ReceiveMessage", async (string user, string message) =>
            {
                string a = user;
                DependencyService.Get<IMessage>().ShortAlert(user +" - "+ message);
            });


            hubConexion.On("CartaCompromisoCovid", async (string mensaje,string token) =>
            {
                try
                {
                    RequestEventosMSM request = JsonConvert.DeserializeObject<RequestEventosMSM>(mensaje);
                    CartaCompromisoCovid19 carta = JsonConvert.DeserializeObject<CartaCompromisoCovid19>(request.JsonDatos);
                    carta.TipoFirma = "CartaCompromisoCovid";
                    App.Current.Properties["name"] = request.Usuario;
                    App.Current.Properties["token"] = token;
                    
                    App.Current.Properties["userName"] = request.Usuario;
                    //Revisar
                    await Navigation.PushModalAsync(new CartaCompromiso(this._service, this._serviceCarta, this._serviceConf, carta));


                }catch(Exception e)
                {
                    DependencyService.Get<IMessage>().ShortAlert(e.Message);
                }
                

            });

            hubConexion.On("InstruccionesEgreso", async (string mensaje,string token) =>
            {
                try
                {
                    RequestEventosMSM request = JsonConvert.DeserializeObject<RequestEventosMSM>(mensaje);
                    FirmaPaciente firma = JsonConvert.DeserializeObject<FirmaPaciente>(request.JsonDatos);
                    firma.tipo = "InstruccionesEgreso";
                    App.Current.Properties["name"] = request.Usuario;
                    App.Current.Properties["token"] = token;

                    App.Current.Properties["userName"] = request.Usuario;
                    //Revisar
                    await Navigation.PushModalAsync(new InstruEgresoFirma(this._service, this._serviceInsEgreso, this._serviceConf, firma));


                }
                catch (Exception e)
                {
                    DependencyService.Get<IMessage>().ShortAlert(e.Message);
                }


            });

            hubConexion.On("HabeasData", async (string mensaje, string token) =>
            {
                try
                {
                    
                    RequestEventosMSM request = JsonConvert.DeserializeObject<RequestEventosMSM>(mensaje);
                    var firma = JsonConvert.DeserializeObject<HabeasDataEvent>(request.JsonDatos);
                   // firma.tipo = "InstruccionesEgreso";
                    App.Current.Properties["name"] = request.Usuario;
                    App.Current.Properties["token"] = token;

                    App.Current.Properties["userName"] = request.Usuario;
                    
                    CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo(firma.Idioma);
                    await Navigation.PushModalAsync(new ConstanciaHabeasDataEvent(firma));
                    

                }
                catch (Exception e)
                {
                    DependencyService.Get<IMessage>().ShortAlert(e.Message);
                }


            });


        }

        public async Task ConnectAsync()
        {
           
            try 
            {

                if(hubConexion.State == HubConnectionState.Disconnected)
                {
                    await hubConexion.StartAsync();
                }
                if ((hubConexion.State == HubConnectionState.Connecting) || (hubConexion.State == HubConnectionState.Reconnecting) )
                {
                    DependencyService.Get<IMessage>().ShortAlert("Conectando");
                }

                if ((hubConexion.State == HubConnectionState.Disconnected) )
                {
                    DependencyService.Get<IMessage>().ShortAlert("Desconectado");
                }

                if ((hubConexion.State == HubConnectionState.Connected))
                {
                    txtConexion.Text = "Conectado";
                    txtConexion.IsVisible = true;
                    txtConexion.TextColor = Color.DarkGreen;
                    this.hubConectado = true;
                    DependencyService.Get<IMessage>().ShortAlert("Conectado");
                   hubConexion.Closed += Connection_Closed;
                }


            }
            catch (Exception e)
            {
                DependencyService.Get<IMessage>().ShortAlert(e.Message);
                this.hubConectado = false;
            }
            
        }

        
        private async Task Connection_Closed(Exception e)
        {   // A global variable being set in "Form_closing" event 
            // of Form, check if form not closed explicitly to prevent a possible deadlock.
            Device.BeginInvokeOnMainThread(SomeMethod);

        }

        private async void SomeMethod()
        {
            try
            {
                this.hubConectado = false;
                hubConexion.Closed -= Connection_Closed;
                txtConexion.Text = "Reconectando";
                txtConexion.TextColor = Color.Red;
                // specify a retry duration
                TimeSpan retryDuration = TimeSpan.FromSeconds(30);
                DateTime retryTill = DateTime.UtcNow.Add(retryDuration);

                while (DateTime.UtcNow < retryTill)
                {
                    await ConnectAsync();
                    if (this.hubConectado)
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error :: " + ex.Message);
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                await hubConexion.StopAsync();
                this.hubConectado = false;

                if ((hubConexion.State == HubConnectionState.Disconnected))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Desconectado");
                    txtConexion.Text = "";
                }

                if ((hubConexion.State == HubConnectionState.Connected))
                {
                    DependencyService.Get<IMessage>().ShortAlert("No ha sido posible Desconectar");
                }

            }
            catch (Exception e)
            {
                this.hubConectado = false;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DisconnectAsync();
        }
    }
}