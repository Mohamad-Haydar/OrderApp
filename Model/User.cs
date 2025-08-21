using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderApp.Model
{
    public partial class User : ObservableObject
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [ObservableProperty]
        public string email;
        public string Password{ get; set; }

        [ObservableProperty]
        string emailError;
        partial void OnEmailChanged(string value)
        {
            EmailError = string.Empty;
        }
        public bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                EmailError = "Please enter a valid email address.";
                return false;
            }
            else
                EmailError = string.Empty;
            return true;
        }

        private bool IsValidEmail(string email)
        {
            // Simple regex for demonstration; you can use a more strict one if needed
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
