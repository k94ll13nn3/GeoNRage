using System;
using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class PopupRender : IDisposable
    {
        private bool _disposedValue;

        [Inject]
        public PopupService PopupService { get; set; } = null!;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            if (PopupService is null)
            {
                throw new InvalidOperationException("Cannot start Popup component.");
            }

            PopupService.OnPopupUpdated += Update;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && PopupService is not null)
                {
                    PopupService.OnPopupUpdated -= Update;
                }

                _disposedValue = true;
            }
        }

        private void Update(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        private void OnValidate()
        {
            PopupService.OnOnClick?.Invoke();
            PopupService.HidePopup();
        }
    }
}
