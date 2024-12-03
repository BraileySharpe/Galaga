using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Galaga.ViewModel;

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
            DataContext = this.gameViewModel = new GameViewModel(this.canvas);
            this.gameViewModel.PropertyChanged += this.OnViewModelPropertyChanged;

            this.setupWindowPreferences();

            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;
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

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.gameViewModel.HasLost) && this.gameViewModel.HasLost)
            {
                this.endGame("GAME OVER");
            }

            if (e.PropertyName == nameof(this.gameViewModel.HasWon) && this.gameViewModel.HasWon)
            {
                this.endGame("YOU WIN!");
            }
        }

        private void endGame(string endgameText)
        {
            CompositionTarget.Rendering -= (sender, args) => this.gameViewModel.UpdateGameState();
            this.disableAllSprites();
            this.gameViewModel.StopAllTimers();

            this.endGameTextBlock.Text = endgameText;
            this.endGameTextBlock.Visibility = Visibility.Visible;
        }

        private void disableAllSprites()
        {
            foreach (var uiElement in this.canvas.Children)
            {
                if (!(uiElement is TextBlock))
                {
                    uiElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}