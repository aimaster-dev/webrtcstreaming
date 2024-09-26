using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using webrtcstreaming;
using webrtcstreaming.Platforms.Android;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using webrtcstreaming.Platforms.Android.Renderer;

[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer))]
namespace webrtcstreaming.Platforms.Android.Renderer
{
    public class CustomPickerRenderer : PickerRenderer
    {
        public CustomPickerRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null) 
            {
                Control.Background = null;
            }
        }
    }
}
