using InteliVision.Views;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InteliVision
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //if (Device.RuntimePlatform == Device.iOS)
            //    MainPage = new MainPage();
            //else
            //    MainPage = new NavigationPage(new MainPage());

            MainPage = new NavigationPage(new MainMenuPage());
        }
    }
}