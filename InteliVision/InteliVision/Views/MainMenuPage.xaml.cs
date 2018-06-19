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
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
             
        }

        private void btnText_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AboutPage());
        }

        private void btnCurrency_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PictorialPage());
        }

        private void btnObject_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ObjectPage());
        }
    }
}