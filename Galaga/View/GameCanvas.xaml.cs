using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Galaga.ViewModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Galaga.View
{
    public sealed partial class GameCanvas
    {
        #region Data members

        private readonly GameViewModel gameViewModel;

        #endregion

        #region Constructors

        public GameCanvas()
        {
            this.InitializeComponent();
            this.setupWindowPreferences();
            this.gameViewModel = new GameViewModel(this.canvas);

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            DataContext = this.gameViewModel;
            this.gameViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
        }

        #endregion

        #region Methods

        private void setupWindowPreferences()
        {
            Width = this.canvas.Width;
            Height = this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            this.gameViewModel.KeyDown(args.VirtualKey);
        }

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            this.gameViewModel.KeyUp(args.VirtualKey);
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(this.gameViewModel.HasLost) && this.gameViewModel.HasLost) || e.PropertyName == nameof(this.gameViewModel.HasWon) && this.gameViewModel.HasWon)
            {
                string playerName = await this.promptForPlayerNameAsync();
                await this.gameViewModel.EndGameAsync(playerName);
                await this.ShowHighScoreBoardAsync();
            }
        }

        private async Task<string> promptForPlayerNameAsync()
        {
            var dialog = new ContentDialog
            {
                Title = "Enter Your Name",
                Content = new TextBox { PlaceholderText = "Player Name" },
                PrimaryButtonText = "OK",
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return ((TextBox)dialog.Content).Text;
            }

            return "Anonymous";
        }

        #endregion
    }
}