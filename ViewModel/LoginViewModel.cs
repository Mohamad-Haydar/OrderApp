using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace OrderApp.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        User user;

        [ObservableProperty]
        public bool enableBiometrics;

        public LocalizationService _localization;
        public ThemeService _themeService;
        public LoginServices _loginServices;

        public Helpers _helper;

        SqliteConnection connection;
        public readonly IFingerprint _fingerprint;


        public LoginViewModel() 
        {
            user = new() { UserName = "admin", Email= "admin@gmail.com", Password="1234"};
            EnableBiometrics = Preferences.Get("BiometricEnabled", false);
            connection = AdoDatabaseService.GetConnection();
            _fingerprint = ServiceHelper.Resolve<IFingerprint>();
            _helper = ServiceHelper.Resolve<Helpers>();
            _localization = ServiceHelper.Resolve<LocalizationService>();
            _themeService = ServiceHelper.Resolve<ThemeService>();
            _loginServices = ServiceHelper.Resolve<LoginServices>();
        }

        [RelayCommand]
        async Task RegisterAdminAsync()
        {
            const string adminUsername = "admin";
            const string adminEmail = "admin@gmail.com";
            const string plainPassword = "1234";
            string hashedPassword = _helper.ComputeSha256Hash(plainPassword);

            try
            {
                connection.Open();
                // Check User if already present in the database
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT COUNT(*) 
                    FROM Users 
                    WHERE Username = $username OR Email = $email";

                checkCommand.Parameters.AddWithValue("$username", adminUsername);
                checkCommand.Parameters.AddWithValue("$email", adminEmail);

                var exists = (long)checkCommand.ExecuteScalar();
                // if the user already exists show an error
                if (exists > 0)
                {
                    await Shell.Current.DisplayAlert("Info", "Admin user already exists.", "OK");
                    return;
                }
                // when the user is not present in the database, create new user
                using var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"INSERT INTO Users (Username, Email, Password)
                    VALUES ($username, $email, $password)";

                insertCommand.Parameters.AddWithValue("$username", adminUsername);
                insertCommand.Parameters.AddWithValue("$email", adminEmail);
                insertCommand.Parameters.AddWithValue("$password", hashedPassword);

                var rows = insertCommand.ExecuteNonQuery();

                await Shell.Current.DisplayAlert("Success", "Admin user created successfully.", "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while registering the admin. Please try again.", "OK");
            }
            finally
            {
                connection.Close();
            }
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            try
            {
                // try to login with biometric
                var res = await TryAutoLoginWithBiometricsAsync();
                if(res)
                {
                    Application.Current.MainPage = new AppShell(new ShellViewModel(Language));
                    // make sure that the language is set in the new shell
                    _localization.SetLanguage(Language);
                    return;
                }

                // in case biometric don't work or is not enable normal login
                if (string.IsNullOrWhiteSpace(User?.UserName) ||
                       string.IsNullOrWhiteSpace(User?.Email) ||
                       string.IsNullOrWhiteSpace(User?.Password))
                {
                    if (EnableBiometrics)
                    {
                        await Shell.Current.DisplayAlert("Error", "Please fill all fields At Least once before using biometrics login", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Please fill all fields", "OK");
                    }
                    return;
                }

                IsBusy = true;
                bool LoginResult = false;

               
                Preferences.Set("BiometricEnabled", EnableBiometrics);
                LoginResult = await _loginServices.Login(User);
                if (LoginResult)
                {
                    await SecureStorage.SetAsync("SavedEmail", User.Email);
                    await SecureStorage.SetAsync("SavedPassword", User.Password);

                    IsBusy = false;
                    Application.Current.MainPage = new AppShell(new ShellViewModel(Language));
                    _localization.SetLanguage(Language);
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred during login. Please try again.", "OK");
                IsBusy = false;
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SwitchLanguageAsync()
        {
            await base.SwitchLanguageAsync();
        }

        public async Task<bool> TryAutoLoginWithBiometricsAsync()
        {
            if (!EnableBiometrics)
                return false;

            var username = Preferences.Get("Username", null);
            //var userId = Preferences.Get("UserId", null);
            var email = await SecureStorage.GetAsync("SavedEmail");
            var password = await SecureStorage.GetAsync("SavedPassword");

            if (string.IsNullOrWhiteSpace(username)  || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return false;

            var success = await AuthenticateWithBiometricsAsync("Authenticate to login");
            if (success)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AuthenticateWithBiometricsAsync(string reason)
        {
            var result = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Login", reason));
            return result.Authenticated;
        }


    }
}
