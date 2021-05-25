using System;

namespace GeoNRage.App.Services
{
    public class PopupService
    {
        public event EventHandler? OnPopupUpdated;

        public bool IsOpen { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string Message { get; private set; } = string.Empty;

        public void DisplayPopup(string title, string message)
        {
            IsOpen = true;
            Title = title;
            Message = message;
            OnPopupUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void HidePopup()
        {
            IsOpen = false;
            Title = string.Empty;
            Message = string.Empty;
            OnPopupUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
