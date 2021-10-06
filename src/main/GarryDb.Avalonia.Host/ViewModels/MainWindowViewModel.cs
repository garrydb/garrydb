using System.Reactive;

using GarryDb.Avalonia.Host.Views;

using ReactiveUI;

using Splat;

namespace GarryDb.Avalonia.Host.ViewModels
{
    public sealed class MainWindowViewModel : ReactiveObject, IScreen
    {
        public MainWindowViewModel()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.Register(() => new FirstView(), typeof(IViewFor<FirstViewModel>));
            Locator.CurrentMutable.Register(() => new ChessboardView(), typeof(IViewFor<ChessboardViewModel>));

            // Manage the routing state. Use the Router.Navigate.Execute
            // command to navigate to different view models. 
            //
            // Note, that the Navigate.Execute method accepts an instance 
            // of a view model, this allows you to pass parameters to 
            // your view models, or to reuse existing view models.
            //
            GoNext = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new FirstViewModel(this)));
        }

        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, IRoutableViewModel?> GoBack
        {
            get { return Router.NavigateBack; }
        }

        // The Router associated with this Screen.
        // Required by the IScreen interface.
        public RoutingState Router { get; }
    }
}
