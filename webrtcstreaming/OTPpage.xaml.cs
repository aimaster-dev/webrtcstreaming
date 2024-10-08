using Newtonsoft.Json;

namespace webrtcstreaming;

public class CheckOTP
{
    public bool status { get; set; }
    public required string data { get; set; }
}

public partial class OTPpage : ContentPage
{
	public OTPpage()
	{
		InitializeComponent();
	}
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
    private async void OnResendTapped(object sender, EventArgs e)
    {
        
    }
    private async void OtpTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue) || e.NewTextValue.Length == 0)
        {
            return;
        }
        if (e.NewTextValue.Length == 1)
        {
            if (sender == otp1) otp2.Focus();
            else if (sender == otp2) otp3.Focus();
            else if (sender == otp3) otp4.Focus();
            else if (sender == otp4) otp5.Focus();
            else if (sender == otp5) otp6.Focus();
        }

        if (otp1.Text?.Length == 1 && otp2.Text?.Length == 1 && otp3.Text?.Length == 1 && otp4.Text?.Length == 1 && otp5.Text?.Length == 1 && otp6.Text?.Length == 1) 
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string enteredOtp = $"{otp1.Text}{otp2.Text}{otp3.Text}{otp4.Text}{otp5.Text}{otp6.Text}";
                    string check_url = $"https://api.emmysvideos.com/api/v1/user/phone/activate/{enteredOtp}";
                    var response = await client.GetAsync(check_url);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CheckOTP>(responseString);
                        FooterMsg.Text = responseData.data;
                        ResendTap.Text = "";
                    }
                    else
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CheckOTP>(responseString);
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            FooterMsg.Text = responseData.data;
                            ResendTap.Text = "";
                        }
                        else 
                        {
                            await DisplayAlert("Failed", $"{response.Content}", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to sign in: {ex.Message}", "OK");
            }
        }
    }
}