﻿using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;

namespace webrtcstreaming
{
    public class TourPlace
    {
        public int id { get; set; }
        public required string place_name { get; set; }
        public int isp { get; set; }

        public override string ToString()
        {
            return place_name;
        }
    }

    public class TourPlaceAPIResponse
    {
        public bool status { get; set; }
        public required List<TourPlace> data { get; set; }
    }

    public class Credential
    {
        public required string refresh { get; set; }
        public required string access { get; set; }
        public int user_id { get; set; }
        public int usertype { get; set; }
        public int level { get; set; }
        public required string username { get; set; }
        public bool status { get; set; }
        public List<int?> tourplace { get; set; }

    }

    public class SigninAPIResponse
    {
        public bool status { get; set; }
        public Credential data { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        int count = 0;
        public ObservableCollection<TourPlace> TourPlaceOptions { get; set; }

        public TourPlace SelectedTourPlace { get; set; }
        public MainPage()
        {
            InitializeComponent();

            TourPlaceOptions = new ObservableCollection<TourPlace>();

            BindingContext = this;

            _ = LoadTourPlacesFromServer();
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

        private async Task LoadTourPlacesFromServer()
        {
            string apiUrl = "https://api.emmysvideos.com/api/v1/tourplace/getall";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(apiUrl);
                    var apiResponse = JsonConvert.DeserializeObject<TourPlaceAPIResponse>(response);
                    TourPlaceOptions.Add(new TourPlace
                    {
                        id = -1,  // Use a special ID to indicate "None"
                        place_name = "None",
                        isp = -1
                    });
                    if (apiResponse != null && apiResponse.status && apiResponse.data != null)
                    {
                        foreach (var place in apiResponse.data)
                        {
                            TourPlaceOptions.Add(place);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Error", $"Failed to load tour places: {ex.Message}", "OK");
            }
        }

        private async void OnSigninClicked(object sender, EventArgs e)
        {
            string email = EmailAddress.Text;
            string password = Password.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            if(!IsValidEmail(email))
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            int? selectedTourPlaceId = null;
            if (SelectedTourPlace != null && SelectedTourPlace.id != -1) // If "None" is selected, treat as no selection
            {
                selectedTourPlaceId = SelectedTourPlace.id;
            }

            var requestPayload = new
            {
                email = email,
                password = password,
                tourplace = SelectedTourPlace?.id
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://api.emmysvideos.com/api/v1/user/login";
                    string jsonPayload = JsonConvert.SerializeObject(requestPayload);

                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<SigninAPIResponse>(responseString);

                        await SecureStorage.SetAsync("AccessToken", responseData.data.access);
                        await SecureStorage.SetAsync("RefreshToken", responseData.data.refresh);

                        await DisplayAlert("Login Successful", $"Refresh Token: {responseData.data.refresh}\nAccess Token: {responseData.data.access}", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Login failed. Please check your credentials.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to sign in: {ex.Message}", "OK");
            }
        }

        private async void OnSignupTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("///signup");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

}
