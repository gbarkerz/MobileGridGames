using MobileGridGames.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WheresGameSettingsPage : ContentPage
    {
        public WheresGameSettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new WheresSettingsViewModel();

            var vm = this.BindingContext as WheresSettingsViewModel;
            vm.PlaySoundOnMatch = Preferences.Get("WheresPlaySoundOnMatch", true);
            vm.PlaySoundOnNotMatch = Preferences.Get("WheresPlaySoundOnNotMatch", true);
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}