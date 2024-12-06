using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Galaga.ViewModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;

namespace Galaga.View
{
    public sealed partial class GameCanvas
    {
        #region Data members

        private readonly GameViewModel gameViewModel;

        private const double BackgroundSpeed = 1.0;
        private double backgroundTopPosition = 0;
        private double backgroundBottomPosition = 325;

        #endregion

        #region Constructors

        public GameCanvas()
        {
            this.InitializeComponent();
            this.setupWindowPreferences();
            this.gameViewModel = new GameViewModel(this.canvas, this.UpdateParallaxBackground);

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

            DataContext = this.gameViewModel;
            this.gameViewModel.PropertyChanged += this.OnViewModelPropertyChanged;

            _ = this.gameViewModel.LoadHighScoresAsync();
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
                this.gameViewModel.EndGame();
                string playerName = await this.promptForPlayerNameAsync();
                await this.gameViewModel.EndGameAsync(playerName);
                this.highScoreBoardListView.Visibility = Visibility.Visible;
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
                var name = ((TextBox)dialog.Content).Text;
                if (name.Length > 0)
                {
                    return name;
                }
                return "Rico";
            }

            return "Anonymous";
        }

        private void UpdateParallaxBackground()
        {
            Debug.WriteLine("Updating Parallax Background");
            Debug.WriteLine("BackgroundTopPosition: " + backgroundTopPosition);
            Debug.WriteLine("BackgroundBottomPosition: " + backgroundBottomPosition);
            backgroundTopPosition += BackgroundSpeed;
            backgroundBottomPosition += BackgroundSpeed;

            if (backgroundTopPosition >= 650)
            {
                backgroundTopPosition = -325;
            }

            if (backgroundBottomPosition >= 650)
            {
                backgroundBottomPosition = -325;
            }

            Canvas.SetTop(BackgroundTop, backgroundTopPosition);
            Canvas.SetTop(BackgroundBottom, backgroundBottomPosition);

            #endregion
        }
    }
}