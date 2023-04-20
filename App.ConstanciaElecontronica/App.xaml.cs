using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Frame;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Process;
using App.ConstanciaElecontronica.Services;
using App.ConstanciaElecontronica.SignalR;
using Plugin.Multilingual;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App.ConstanciaElecontronica
{
    public partial class App : Application, ILoginService
    {
        

        public static App Current;
        public static string AppId=Secrets.IdApplication;
        public static string Api = Secrets.APILogin;
        private object mainActivity;
        private readonly ServicePatient _service = new ServicePatient(Secrets.APIConstancia);
        private Loading load;
        public App(object Activity)
        {
            CrossMultilingual.Current.CurrentCultureInfo=new CultureInfo("es");
            load =new Loading();
            mainActivity = Activity;

            InitializeComponent();
            Current = this;
            MainPage = load;      // ShowMainPage();


        }
        public Task Initialization { get; private set; }
        
        protected override void OnStart()
        {
            //  MethodInfo method = mainActivity.GetType().GetMethod("validateVersion");
            // method.Invoke(mainActivity, null);
            // ShowMainPage();
            Initialization = InitializeAsync();          

        }
        private async Task InitializeAsync()
        {
            // Asynchronously wait for the fundamental instance to initialize.            
            // Do our own initialization (synchronous or asynchronous).
            await Task.Delay(500);

            load.run();
          
            var r =await verified();
            if (!string.IsNullOrEmpty(r)) { 
                MethodInfo method = mainActivity.GetType().GetMethod("validateVersion");
                 method.Invoke(mainActivity, new object[]{ r });
            }            ShowMainPage();
        }

        private async Task<string> verified() {
            //var folder = Android.OS.Environment.DirectoryDownloads;
            try
            {
                
                  MethodInfo method = mainActivity.GetType().GetMethod("CheckAppPermissions");
                method.Invoke(mainActivity, null);

                var version = VersionTracking.CurrentVersion;
                var api = Secrets.APILogin;
                var appId = Secrets.IdApplication;
                var service = new LoginService(api);
                load.run();
                var path = "/sdcard/download";
                path += $"/com.companyname.app.constanciaelecontronica{version}.apk";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                load.run();

                var ResponseAppInfo = await service.GetApplicationInfo(appId);

                load.run();
                if (ResponseAppInfo.code != 200)
                {
                    DependencyService.Get<IMessage>().LongAlert("Error inesperado :: " + ResponseAppInfo.message);
                    return null;
                }
                var appInfo = ResponseAppInfo.data;
                try {
                    var v1=Convert.ToDouble(version.Replace('.',','));
                    var v2 = Convert.ToDouble(appInfo.Version.Replace('.', ','));
                    if (v1 >= v2) return null;
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return null;
                }
               
                load.showVersion();
                load.run();

              

                load.showDownload();
                var pathDownload = "/sdcard/download";
                pathDownload += $"/com.companyname.app.constanciaelecontronica{appInfo.Version}.apk";
                if (!File.Exists(pathDownload)) { 
                    var filesb = await service.getFile(appId);
                    load.run();
                    if (filesb == null) return null;
                    System.IO.File.WriteAllBytes(pathDownload, filesb);
                }                
                return pathDownload;
            }
            catch (Exception ex) {
                DependencyService.Get<IMessage>().LongAlert("Error inesperado :: " + ex.Message);
                return null;
            }
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private static Page inicio = null;
        public void ShowMainPage(string atencion)
        {
            ((Inicio) inicio).loadData(atencion);
            ShowMainPage();
        }
        public async void ShowMainPage()
        {
            /* lista de roles
             * HabeasData
             * Instrucciones
             * CuidadosEnfermeria           
             */
            var isLoggedIn = Properties.ContainsKey("IsLoggedIn") ? (bool)Properties["IsLoggedIn"] : false;
            if (isLoggedIn)
            {
                MainPage = new MasterDetailPage1();
            }
            else
            {

                var service = new ServiceCartaCompromiso(Secrets.APIConstancia);
                var response = await  service.ValidarTablet();
                if (response.code == 200)
                {
                    ServiceCartaCompromiso serviceCarta = new ServiceCartaCompromiso(Secrets.APIConstancia);
                    ServiceInstruccionesEgreso serviceInsEgreso = new ServiceInstruccionesEgreso(Secrets.APIConstancia);
                    ServiceConfiguracion serviceConf = new ServiceConfiguracion(Secrets.APIConstancia);

                    MainPage = new InicioSolicitud(this._service, serviceCarta, serviceInsEgreso, serviceConf, Secrets.APIEventos);
                }
                else
                {
                    MainPage = new Login(this);
                }


            }
            

        }

        public void Logout()
        {
            Properties["IsLoggedIn"] = false;
            Xamarin.Forms.Application.Current.Properties.Clear();
             inicio = null;
            MainPage = new Login(this);
        }
    }
}
