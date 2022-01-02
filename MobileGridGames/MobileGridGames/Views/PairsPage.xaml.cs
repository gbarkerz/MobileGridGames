using MobileGridGames.Models;
using MobileGridGames.ViewModels;
using MobileGridGames.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    public partial class PairsPage : ContentPage
    {
        SettingsViewModel _viewModel;

        public PairsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SettingsViewModel();
        }
    }
}