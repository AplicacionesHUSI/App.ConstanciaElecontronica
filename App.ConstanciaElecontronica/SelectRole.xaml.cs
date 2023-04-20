using App.ConstanciaElecontronica.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.ConstanciaElecontronica
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectRole : ContentPage
    {
        private readonly MasterDetailPage1 _page;
        public SelectRole(MasterDetailPage1 Page, List<string> Roles)
        {
            InitializeComponent();
            _page = Page;
            foreach (var rol in Roles)
            {
                var name = Util.Util.GetNameMenu(rol);
                var btn = new Button() { Text = name,ClassId=rol ,AnchorX=180,CornerRadius=10, HorizontalOptions = LayoutOptions.CenterAndExpand };
                btn.Clicked += BtnAllClick;
                layoutMenu.Children.Add(btn); ;
            }


        }

         void  BtnAllClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            App.Current.Properties["role"] = btn.ClassId;
            _page.SetPage(btn.ClassId);
        }
    }
}