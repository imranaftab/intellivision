using InteliVision.Views;
using System;

using Xamarin.Forms;

namespace InteliVision
{
    public class MainPage : TabbedPage
    {
        public MainPage()
        {
            Page itemsPage, aboutPage, objectPage = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    itemsPage = new NavigationPage(new PictorialPage())
                    {
                        Title = "Currency"
                    };

                    aboutPage = new NavigationPage(new AboutPage())
                    {
                        Title = "Textual"
                    };
                    itemsPage.Icon = "tab_feed.png";
                    aboutPage.Icon = "tab_about.png";
                    break;
                default:
                    itemsPage = new PictorialPage()
                    {
                        Title = "Currency"
                    };

                    aboutPage = new AboutPage()
                    {
                        Title = "Text"
                    };
                    objectPage = new ObjectPage
                    {
                        Title = "Object"
                    };


                    break;
            }

            Children.Add(aboutPage);
            Children.Add(itemsPage);
            Children.Add(objectPage);
            NavigationPage.SetHasNavigationBar(this, false);
            Title = Children[0].Title;
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage?.Title ?? string.Empty;
        }
    }
}
