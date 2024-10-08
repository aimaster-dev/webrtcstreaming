namespace webrtcstreaming;

public partial class Video : ContentPage
{
	public Video(Camera camera)
	{
        InitializeComponent();

        cameraView.Url = $"rtsp://{camera.camera_ip}:{camera.camera_port}";
        cameraView.User = camera.camera_user_name;
        cameraView.Password = camera.password;
    }
}