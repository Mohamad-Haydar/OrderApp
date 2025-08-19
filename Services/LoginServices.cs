using OrderApp.Exceptions;
using OrderApp.Helper;
using OrderApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp.Services
{
    public class LoginServices
    {
        public Helpers _helper;

        public LoginServices(Helpers helper)
        {
            _helper = helper;
        }

        public async Task<bool> Login(User user)
        {
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await connection.OpenAsync();
                // check for the credentials in the database
                using var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Users 
                                    WHERE Username = $username AND Email = $email AND Password = $password";
                var hashedPassword = _helper.ComputeSha256Hash(user.Password.Trim()); // use hashed password

                command.Parameters.AddWithValue("$username", user.UserName.Trim());
                command.Parameters.AddWithValue("$email", user.Email.Trim());
                command.Parameters.AddWithValue("$password", hashedPassword);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int userId = reader.GetInt32(0);
                    string username = reader.GetString(1);
                    string email = reader.GetString(2);
                    string password = reader.GetString(3);

                    // store user ID and userName in Preferences
                    Preferences.Set("UserId", userId);
                    Preferences.Set("Username", username);
                    await SecureStorage.SetAsync("SavedEmail", email);
                    await SecureStorage.SetAsync("SavedPassword", password);
                    await connection.CloseAsync();
                    return true;
                }
                else
                {
                    await connection.CloseAsync();
                    // No user Is found in the database or credentials are false
                    await Shell.Current.DisplayAlert("Login Failed", "Invalid credentials", "OK");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in Login: {ex}");
                throw new DataAccessException("Could not Login", ex);
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
            }
        }
    }
}
