using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.ViewModel;

namespace Galaga.View;

public sealed partial class GameCanvas
{
    #region Data members

    private const double BackgroundSpeed = 1.0;

    private readonly GameViewModel gameViewModel;
    private double backgroundTopPosition;
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

        this.backgroundTopPosition = 0;
        this.backgroundBottomPosition = this.backgroundTopPosition + this.backgroundTop.Height;

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
        if ((e.PropertyName == nameof(this.gameViewModel.HasLost) && this.gameViewModel.HasLost) ||
            (e.PropertyName == nameof(this.gameViewModel.HasWon) && this.gameViewModel.HasWon))
        {
            this.gameViewModel.EndGame();
            var playerName = await this.promptForPlayerNameAsync();
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
            PrimaryButtonText = "OK"
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
        this.backgroundTopPosition += BackgroundSpeed;
        this.backgroundBottomPosition += BackgroundSpeed;

        if (this.backgroundTopPosition >= this.canvas.Height)
        {
            this.backgroundTopPosition = this.backgroundBottomPosition - this.backgroundBottom.Height;
        }

        if (this.backgroundBottomPosition >= this.canvas.Height)
        {
            this.backgroundBottomPosition = this.backgroundTopPosition - this.backgroundTop.Height;
        }

        Canvas.SetTop(this.backgroundTop, this.backgroundTopPosition);
        Canvas.SetTop(this.backgroundBottom, this.backgroundBottomPosition);

        #endregion
    }
}