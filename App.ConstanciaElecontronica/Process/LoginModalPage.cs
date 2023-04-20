using Xamarin.Forms;

namespace App.ConstanciaElecontronica.Process
{
    public class LoginModalPage : CarouselPage
    {
        ContentPage login;

        public LoginModalPage(ILoginService ilm) {
            login = new Login(ilm);

            this.Children.Add(login);
            MessagingCenter.Subscribe<ContentPage>(this, "Login", (sender) => { this.SelectedItem = login; });
        }

    }
}
