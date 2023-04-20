using App.ConstanciaElecontronica.Entities;
using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Helpers;
using App.ConstanciaElecontronica.Process;
using App.ConstanciaElecontronica.Services;
using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private readonly string _ApplicationId;

        private readonly ILoginService loginService;
        private readonly LoginService _service = new LoginService(Secrets.APILogin);
        public Login(ILoginService ilm)
        {

            InitializeComponent();
            loginService = ilm;
            _ApplicationId = Secrets.IdApplication;
            
        }

       private  void btnLoginClick(object sender, EventArgs e)
        {
            var credentials = new Credentials()
            {
                Username = UserName.Text,
                Password = Password.Text,
                ApplicationId = _ApplicationId
            };

            var reponseLogin = _service.ValidationLoggin(credentials);

            if (reponseLogin.code == 200)
            {
                App.Current.Properties["name"] = credentials.Username;
                App.Current.Properties["token"] = reponseLogin.data.Token;
                App.Current.Properties["IsLoggedIn"] = true;
                App.Current.Properties["userName"] = reponseLogin.data.User ;
                var roles = reponseLogin.data.Roles;
                App.Current.Properties["roles"] = reponseLogin.data.Roles;
                App.Current.Properties["role"] = roles[0];
                    RedirecMainPage(roles[0]);
               
            }
            else
            {
                switch (reponseLogin.code) {
                    case 401:
                        msgValidation.Text = "Credenciales invalidas";
                        msgValidation.IsVisible = true;
                        break;

                    case 403:
                        msgValidation.Text = "Acceso denegado";
                        msgValidation.IsVisible = true;
                        break;
                    default:
                        DependencyService.Get<IMessage>().LongAlert("from server :: " + reponseLogin.message);
                        break;
                }
                
            }
            
        }

        public void RedirecMainPage(string Role) {
            App.Current.Properties["role"] = Role;
            loginService.ShowMainPage();
        }


        public void NameCompleted(object sender, EventArgs e)
        {
            Password.Focus();
        }

        public void OnEnterPressed(object sender, EventArgs e)
        {
            btnLoginClick(sender,e);
        }



    }
}