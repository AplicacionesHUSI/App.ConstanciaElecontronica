using Android.Content;
using Android.Text;
using Android.Widget;
using App.ConstanciaElecontronica.Droid;
using App.ConstanciaElecontronica.Util;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(HtmlFormattedLabel), typeof(HtmlFormattedLabelRenderer))]
namespace App.ConstanciaElecontronica.Droid
{
    public class HtmlFormattedLabelRenderer : LabelRenderer
    {
        public HtmlFormattedLabelRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var view = (HtmlFormattedLabel)Element;
            if (Control != null && view != null)
            {
                try
                {
                    Control.JustificationMode = JustificationMode.InterWord;

                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }


            }
        }
    }
}