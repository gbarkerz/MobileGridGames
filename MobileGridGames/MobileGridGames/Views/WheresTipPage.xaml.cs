using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WheresTipPage : ContentPage
    {
        public string wcagName
        {
            get
            {
                return name;
            }
        }

        public string wcagGroupName
        {
            get
            {
                return groupName.ToLower();
            }
        }

        public string wcagGroupNumber
        {
            get
            {
                return groupNumber;
            }
        }

        private string name = "";
        private string groupName = "";
        private string groupNumber = "";

        public WheresTipPage(string wcagName, string wcagGroup,string number)
        {
            name = wcagName;
            groupName = wcagGroup;
            groupNumber = number;

            InitializeComponent();

            BindingContext = this;
        }

        private async void WheresTipCloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void LearnMoreButton_Clicked(object sender, EventArgs e)
        {
            // Launcher.OpenAsync is provided by Xamarin.Essentials.
            await Launcher.OpenAsync("https://www.w3.org/TR/WCAG21/#" + groupName.ToLower());
        }
    }
}