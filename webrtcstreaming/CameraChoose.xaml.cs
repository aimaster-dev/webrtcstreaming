using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace webrtcstreaming;
public class Camera
{
    public int id { get; set; }
    public required string camera_name { get; set; }
    public string camera_ip { get; set; }
    public string camera_port { get; set; }
    public string camera_user_name { get; set; }
    public string password { get; set; }
    public string output_url { get; set; }
    public List<TourPlace> tourplace { get; set; }
    public string created_at { get; set; }
    public string updated_at { get; set; }
}

public class GetAllCamAPIResponse
{
    public bool status { get; set; }
    public List<Camera> data { get; set; }
}

public partial class CameraChoose : ContentPage
{
    public ObservableCollection<Camera> Cameras { get; set; }
    public CameraChoose()
	{
		InitializeComponent();
        Cameras = new ObservableCollection<Camera>();
        BindingContext = this;
        LoadCameras();
    }
    private async void LoadCameras()
    {
        await LoadCamerasAsync();
    }
    public async Task<string> GettokenAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("AccessToken");
            if (token != null)
            {
                return token;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving the token: {ex.Message}");
            return null;
        }
    }
    public async Task<List<Camera>> LoadCamerasAsync()
    {
        string token  = await GettokenAsync();
        //await DisplayAlert("token", $"AssessToken: {token}", "OK");
        if (token == null)
        {
            return new List<Camera>();
        }
        var apiUrl = "https://api.emmysvideos.com/api/v1/camera/getall";
        try
        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); ;
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<GetAllCamAPIResponse>(responseBody);
                    if (apiResponse.status)
                    {
                        Cameras.Clear();
                        //await DisplayAlert("Camera", $"CamData: {apiResponse.Data}", "OK");
                        foreach (var camera in apiResponse.data)
                        {
                            Cameras.Add(camera);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"API call failed with status code: {apiResponse.status}");
                    }
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            await DisplayAlert("Error", $"Request error: {httpEx.Message}", "OK");
        }
        catch (TaskCanceledException taskEx)
        {
            await DisplayAlert("Error", "Request timed out or was canceled.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
        return new List<Camera>();
    }
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedCamera = e.CurrentSelection.FirstOrDefault() as Camera;

        if (selectedCamera != null)
        {
            //DisplayAlert("Error", $"Selected Cam Info: {selectedCamera.camera_ip}", "OK");
            await Navigation.PushModalAsync(new NavigationPage(new Video(selectedCamera)));
        }
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}