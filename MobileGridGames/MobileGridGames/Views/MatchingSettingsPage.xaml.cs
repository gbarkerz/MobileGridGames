using MobileGridGames.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchingGameSettingsPage : ContentPage
    {
        public MatchingGameSettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new MatchingSettingsViewModel();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}