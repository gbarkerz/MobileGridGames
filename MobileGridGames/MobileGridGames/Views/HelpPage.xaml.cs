using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        public HelpPage(Page currentPage)
        {
            InitializeComponent();

            if (currentPage is SquaresPage)
            {
                SquaresGameHelpTitle.IsVisible = true;
                SquaresGameHelpContent.IsVisible = true;
            }
            else if (currentPage is MatchingPage)
            {
                MatchingGameHelpTitle.IsVisible = true;
                MatchingGameHelpContent.IsVisible = true;
            }
            else if (currentPage is WheresPage)
            {
                WheresGameHelpTitle.IsVisible = true;
                WheresGameHelpContent.IsVisible = true;
            }
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}