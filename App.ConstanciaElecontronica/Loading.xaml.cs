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
    public partial class Loading : ContentPage
    {
    
        public Loading()
        {
            InitializeComponent();
    
        }

        public void run() {
            bar.IsRunning = true;
        }

        public void showVersion()
        {
            lblNewVersion.IsVisible = true;
        }
        public void showDownload()
        {
            lblDownload.IsVisible = true;
        }
    }
}