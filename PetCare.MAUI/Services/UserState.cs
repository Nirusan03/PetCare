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

        public event Action? OnChange;

        public void SetUser(string token, string email)
        {
            AuthToken = token;
            Email = email;
            NotifyStateChanged();
        }

        public void Logout()
        {
            AuthToken = null;
            Email = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
