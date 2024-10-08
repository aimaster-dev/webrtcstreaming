using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;
namespace webrtcstreaming;

public class SignupAPIResponse
{
    public bool status { get; set; }
    public required string data { get; set; }
}

public partial class Signup : ContentPage
{
    public Signup()
    {
        InitializeComponent();
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        try
        {
            return Regex.IsMatch(email,
                 @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                 RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private int PasswordCheck(string pass,
                              string confirm_pass)
    {
        if (pass.Length < 6)
        {
            return 0;
        }
        else if (pass == confirm_pass) {
            return 1;
        } 
        else
        {
            return 2;
        }
    }

    private void InitTags()
    {
        UserNameField.BorderColor = Color.FromRgba(152, 185, 254, 0.7);
        EmailField.BorderColor = Color.FromRgba(152, 185, 254, 0.7);
        PhoneField.BorderColor = Color.FromRgba(152, 185, 254, 0.7);
        PasswordField.BorderColor = Color.FromRgba(152, 185, 254, 0.7);
        ConfirmPasswordField.BorderColor = Color.FromRgba(152, 185, 254, 0.7);
        UserNameRequiredLabel.IsVisible = false;
        EmailRequiredLabel.IsVisible = false;
        PhoneRequiredLabel.IsVisible = false;
        PasswordRequiredLabel.IsVisible = false;
        ConfirmPasswordRequiredLabel.IsVisible = false;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        string email = EmailAddress.Text;
        string fullName = UserName.Text;
        string phoneNumber = Phone.Text;
        string password = Password.Text;
        string confirm_password = ConfirmPassword.Text;
        InitTags();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            UserNameField.BorderColor = Color.FromRgb(255, 0, 0);
            UserNameRequiredLabel.IsVisible = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            EmailField.BorderColor = Color.FromRgb(255, 0, 0);
            UserNameRequiredLabel.IsVisible = true;
            return;
        }
        if (!IsValidEmail(email)) 
        {
            EmailField.BorderColor = Color.FromRgb(255, 0, 0);
            UserNameRequiredLabel.Text = "Please input your email correctly!";
            UserNameRequiredLabel.IsVisible = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(phoneNumber)) 
        {
            PhoneField.BorderColor = Color.FromRgb(255, 0, 0);
            PhoneRequiredLabel.IsVisible = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordField.BorderColor = Color.FromRgb(255, 0, 0);
            PasswordRequiredLabel.IsVisible = true;
            return;
        }
        if (password.Length < 6)
        {
            PasswordField.BorderColor = Color.FromRgb(255, 0, 0);
            PasswordRequiredLabel.Text = "Password Length should be more than 6";
            PasswordRequiredLabel.IsVisible = true;
        }
        if (password != confirm_password)
        {
            ConfirmPasswordField.BorderColor = Color.FromRgb(255, 0, 0);
            ConfirmPasswordRequiredLabel.Text = "Password and Confirm Password isn't the same. Please check again!";
            ConfirmPasswordRequiredLabel.IsVisible = true;
        }
        var requestPayload = new
        {
            username = fullName,
            email = email,
            phone_number = phoneNumber,
            password = password,
            usertype = 3,
            level = 0
        };
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://api.emmysvideos.com/api/v1/user/phone/register";
                string jsonPayload = JsonConvert.SerializeObject(requestPayload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<SignupAPIResponse>(responseString);

                    if (responseData.status == true) 
                    {
                        await DisplayAlert("Success", $"{responseData.data}", "OK");
                        await Navigation.PushModalAsync(new NavigationPage(new OTPpage()));
                    }
                    else
                    {
                        await DisplayAlert("Failed", $"{responseData.data}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Failed", $"{response.Content}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to sign in: {ex.Message}", "OK");
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
    }
}