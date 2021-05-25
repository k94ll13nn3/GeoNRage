using System;

namespace GeoNRage.App.Services
{
    public class PopupService
    {
        public event EventHandler? OnPopupUpdated;

        public bool IsOpen { get; private set; }

        public bool IsOkCancel { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string Message { get; private set; } = string.Empty;

        public Action? OnOnClick { get; private set; }

        public void DisplayPopup(string title, string message)
        {
            IsOkCancel = false;
            IsOpen = true;
            Title = title;
            Message = message;
            OnPopupUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void DisplayOkCancelPopup(string title, string message, Action onOkClick)
        {
            IsOkCancel = true;
            IsOpen = true;
            Title = title;
            Message = message;
            OnOnClick = onOkClick;
            OnPopupUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void HidePopup()
        {
            IsOkCancel = false;
            IsOpen = false;
            Title = string.Empty;
            Message = string.Empty;
            OnOnClick = null;
            OnPopupUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
