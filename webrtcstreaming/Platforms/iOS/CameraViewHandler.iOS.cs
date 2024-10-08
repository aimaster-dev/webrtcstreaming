#if IOS
using AVKit;
using webrtcstreaming.Controls;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Maui.Rtsp.Handlers
{
    public class CameraViewHandler : ViewHandler<ICameraView, UIView>
    {
        public static IPropertyMapper<ICameraView, CameraViewHandler> Mapper = new PropertyMapper<ICameraView, CameraViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ICameraView.Url)] = MapUrl,
            [nameof(ICameraView.User)] = MapUser,
            [nameof(ICameraView.Password)] = MapPassword
        };

        public CameraViewHandler() : base(Mapper)
        {
        }

        protected override UIView CreatePlatformView() => new UIView();

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);
        }

        public static void MapUrl(CameraViewHandler handler, ICameraView view)
        {
        }
        public static void MapUser(CameraViewHandler handler, ICameraView view)
        {
        }
        public static void MapPassword(CameraViewHandler handler, ICameraView view)
        {
        }
    }
}
#endif