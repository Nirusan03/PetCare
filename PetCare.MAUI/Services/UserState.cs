using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.MAUI.Services
{
    public class UserState
    {
        public string? AuthToken { get; private set;  }
        public string? Email { get; private set; }
        public int UserId { get; set; }

        public event Action? OnChange;

        public void SetUser(string token, string email, int userId)
        {
            AuthToken = token;
            Email = email;
            UserId = userId;
            NotifyStateChanged();
        }

        public void Logout()
        {
            AuthToken = null;
            Email = null;
            UserId = 0;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
