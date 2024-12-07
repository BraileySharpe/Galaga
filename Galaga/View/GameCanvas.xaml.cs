using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.ViewModel;

namespace Galaga.View;

/// <summary>
///     The main game canvas.
/// </summary>
public sealed partial class GameCanvas
{
    #region Data members

    private const double BackgroundSpeed = 1.0;
    private const double MidBackgroundSpeedOffset = .66;
    private const double FarBackgroundSpeedOffset = .33;
    private const double FarBackgroundPositionOffset = .5;
    private const double BluePlanetInitialTopPosition = 800;
    private const double RedPlanetInitialTopPosition = 125;
    private const string DefaultPlayerName = "Rico";
    private const string AnonymousPlayerName = "Anonymous";

    private readonly GameViewModel gameViewModel;
    private double backgroundTopPosition;
    private double backgroundBottomPosition;
    private double backgroundStarsTopPosition;
    private double backgroundStarsBottomPosition;
    private double redPlanetTopPosition;
    private double bluePlanetTopPosition;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameCanvas" /> class.
    /// </summary>
    public GameCanvas()
    {
        this.InitializeComponent();
        this.setupWindowPreferences();
        this.gameViewModel = new GameViewModel(this.canvas, this.updateParallaxBackground);

        Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
        Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;

        DataContext = this.gameViewModel;
        this.gameViewModel.PropertyChanged += this.OnViewModelPropertyChanged;


        this.startGameFlashingAnimation.Begin();
        this.backgroundTopPosition = 0;
        this.backgroundBottomPosition = this.backgroundTopPosition + this.backgroundTop.Height;
        this.backgroundStarsTopPosition = 0;
        this.backgroundStarsBottomPosition = this.backgroundStarsTopPosition + this.backgroundStarsTop.Height;
        this.redPlanetTopPosition = RedPlanetInitialTopPosition;
        this.bluePlanetTopPosition = BluePlanetInitialTopPosition;

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
        if ((e.PropertyName == nameof(this.gameViewModel.HasLost) && this.gameViewModel.HasLost)
            || (e.PropertyName == nameof(this.gameViewModel.HasWon) && this.gameViewModel.HasWon))
        {
            this.gameViewModel.EndGame();
            if (this.gameViewModel.HasScoredHighScore)
            {
                var playerName = await this.promptForPlayerNameAsync();
                await this.gameViewModel.EndGameAsync(playerName);
            }

            this.highScoreBoardListView.Visibility = Visibility.Visible;
        }
    }

    private void displayHighScoreBoard(object sender, RoutedEventArgs e)
    {
        this.gameViewModel.IsScoreBoardOpen = true;
        this.gameViewModel.IsInStartScreen = false;
    }

    private void returnToStart(object sender, RoutedEventArgs e)
    {
        this.gameViewModel.IsScoreBoardOpen = false;
        this.gameViewModel.IsInStartScreen = true;
    }

    private async void resetHighScoreBoard(object sender, RoutedEventArgs e)
    {
        await this.gameViewModel.ResetHighScoreBoard();
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
            return name.Length > 0 ? name : DefaultPlayerName;
        }

        return AnonymousPlayerName;
    }

    private void updateParallaxBackground()
    {
        this.handleNearBackgroundAnimation();
        this.handleMidBackgroundAnimation();
        this.handleFarBackgroundAnimation();
    }

    private void handleNearBackgroundAnimation()
    {
        this.redPlanetTopPosition += BackgroundSpeed;
        this.bluePlanetTopPosition += BackgroundSpeed;

        if (this.redPlanetTopPosition >= this.canvas.Height)
        {
            this.redPlanetTopPosition = -this.redPlanet.Height * 2;
        }

        if (this.bluePlanetTopPosition >= this.canvas.Height)
        {
            this.bluePlanetTopPosition = -this.bluePlanet.Height * 2;
        }

        Canvas.SetTop(this.redPlanet, this.redPlanetTopPosition);
        Canvas.SetTop(this.bluePlanet, this.bluePlanetTopPosition);
    }

    private void handleMidBackgroundAnimation()
    {
        this.backgroundStarsTopPosition += BackgroundSpeed * MidBackgroundSpeedOffset;
        this.backgroundStarsBottomPosition += BackgroundSpeed * MidBackgroundSpeedOffset;

        if (this.backgroundStarsTopPosition >= this.canvas.Height)
        {
            this.backgroundStarsTopPosition = this.backgroundStarsBottomPosition - this.backgroundStarsBottom.Height;
        }

        if (this.backgroundStarsBottomPosition >= this.canvas.Height)
        {
            this.backgroundStarsBottomPosition = this.backgroundStarsTopPosition - this.backgroundStarsTop.Height;
        }

        Canvas.SetTop(this.backgroundStarsTop, this.backgroundStarsTopPosition);
        Canvas.SetTop(this.backgroundStarsBottom, this.backgroundStarsBottomPosition);
    }

    private void handleFarBackgroundAnimation()
    {
        this.backgroundTopPosition += BackgroundSpeed * FarBackgroundSpeedOffset;
        this.backgroundBottomPosition += BackgroundSpeed * FarBackgroundSpeedOffset;

        if (this.backgroundTopPosition >= this.canvas.Height)
        {
            this.backgroundTopPosition = this.backgroundBottomPosition - this.backgroundBottom.Height;
        }

        if (this.backgroundBottomPosition >= this.canvas.Height)
        {
            this.backgroundBottomPosition = this.backgroundTopPosition - this.backgroundTop.Height;
        }

        Canvas.SetTop(this.backgroundTop, this.backgroundTopPosition);
        Canvas.SetTop(this.backgroundBottom, this.backgroundBottomPosition + FarBackgroundPositionOffset);
    }

    #endregion
}