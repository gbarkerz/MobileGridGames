using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        public HelpPage(bool matchingPage)
        {
            InitializeComponent();

            if (matchingPage)
            {
                SquaresGameHelpTitle.IsVisible = false;
                SquaresGameHelpContent.IsVisible = false;
            }
            else
            {
                MatchingGameHelpTitle.IsVisible = false;
                MatchingGameHelpContent.IsVisible = false;
            }
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}