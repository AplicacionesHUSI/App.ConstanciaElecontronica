using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Xamarin.Essentials;
using Java.IO;
using App.ConstanciaElecontronica.Services;
using Android.Widget;
using Android;
using System;

namespace App.ConstanciaElecontronica.Droid
{
    [Activity(Label = "Firma Pacientes", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
         
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

           

            base.OnCreate(savedInstanceState);

            

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(this));
           
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.RequestInstallPackages, PackageName) != Permission.Granted
                    &&  PackageManager.CheckPermission(Manifest.Permission.Camera, PackageName) != Permission.Granted
                    )
                {
                    var permissions = new string[] { Manifest.Permission.RequestInstallPackages,Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.Camera };
                    RequestPermissions(permissions, 1);
                }



            }
        }

        public void validateVersion(string path) {
            try
            {

                File file = new File(path);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    var apkUri = FileProvider.GetUriForFile(this, ApplicationContext.PackageName + ".provider", file);
                    Intent intentS = new Intent(Intent.ActionInstallPackage);
                    intentS.SetData(apkUri);
                    intentS.SetFlags(ActivityFlags.GrantReadUriPermission);
                    StartActivity(intentS);
                }
                else
                {
                    var apkUri = Android.Net.Uri.FromFile(file);
                    Intent intentS = new Intent(Intent.ActionView);
                    intentS.SetDataAndType(apkUri, "application/vnd.android.package-archive");
                    intentS.SetFlags(ActivityFlags.NewTask);
                    StartActivity(intentS);
                }
            }
            catch (Exception e) {
                Toast.MakeText(Application.Context, "Se ha presentado una excepcion :: "+e.Message, ToastLength.Long).Show();
            }
        }
    }
}