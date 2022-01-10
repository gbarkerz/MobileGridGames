using MobileGridGames.ViewModels;
using Xamarin.Forms;

namespace MobileGridGames.Views
{
    public partial class PairsPage : ContentPage
    {
        PairsViewModel _viewModel;

        public PairsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new PairsViewModel();
        }
    }
}