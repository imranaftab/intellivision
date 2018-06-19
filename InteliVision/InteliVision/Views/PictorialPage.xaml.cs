using InteliVision.Common;
using InteliVision.Models;
using InteliVision.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InteliVision.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PictorialPage : ContentPage
    {
        public PictorialPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Subscribe<PictorialViewModel, AlertMessage>(this, Constants.MC_Camera, (s, m) => {
                DisplayAlert(m.Title, m.Message, m.DialogStyle);
            });
        }
        
    }
}