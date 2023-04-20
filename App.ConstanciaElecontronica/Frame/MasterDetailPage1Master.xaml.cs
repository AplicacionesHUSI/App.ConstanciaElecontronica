using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica.Frame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Master : ContentPage
    {
        public ListView ListView;

        public MasterDetailPage1Master()
        {
            InitializeComponent();

            BindingContext = new MasterDetailPage1MasterViewModel();
            ListView = MenuItemsListView;

            var name= App.Current.Properties["userName"].ToString();
            txtUser.Text = App.Current.Properties["name"].ToString()+"@husi.org.co";
            txtNameUsuario.Text = name;
            if (string.IsNullOrEmpty(name)) name = " ";
            txtleter.Text = name[0]+"";
        }

        class MasterDetailPage1MasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailPage1MasterMenuItem> MenuItems { get; set; }

            public MasterDetailPage1MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPage1MasterMenuItem>();
                MenuItems.Add(new MasterDetailPage1MasterMenuItem { Id = "Init", Title = "Inicio" });
                var Roles = (List<string>) App.Current.Properties["roles"];
                foreach (var rol in Roles)
                {
                    var name = Util.Util.GetNameMenu(rol);
                    MenuItems.Add(new MasterDetailPage1MasterMenuItem { Id = rol, Title =name});
                 }
                MenuItems.Add(new MasterDetailPage1MasterMenuItem { Id = "Logout", Title ="Salir" });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
      

    }
}