using System;

using ReactiveUI;

namespace GarryDb.Avalonia.Host.ViewModels
{
    public sealed class FirstViewModel : ReactiveObject, IRoutableViewModel
    {
        public FirstViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        // Reference to IScreen that owns the routable view model.
        public IScreen HostScreen { get; }

        // Unique identifier for the routable view model.
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    }
}
