









using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xamarin.Forms;

namespace InteliVision
{
    public class AboutViewModel : BaseViewModel
    {
        private string _message = "IntelliVision";
        private string _btnMessage = "Recieve Text";
        private ObservableCollection<string> _textSizes = new ObservableCollection<string> { "12", "22", "36", "48", "72"  };
        private int _textSize = 48;
        private int _selectedTextSizeIndex = 3;


        public int SelectedTextSizeIndex
        {
            get { return _selectedTextSizeIndex; }
            set { SetProperty(ref _selectedTextSizeIndex, value, nameof(SelectedTextSizeIndex), () => TextSize = int.Parse(_textSizes[_selectedTextSizeIndex])); }
        }
        
        public ObservableCollection<string> TextSizes => _textSizes;
        public int TextSize
        {
            get { return _textSize; }
            set { SetProperty(ref _textSize, value, nameof(TextSize)); }
        }
        public string BtnMessage
        {
            get { return _btnMessage; }
            set { SetProperty(ref _btnMessage, value, nameof(BtnMessage)); }
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value, nameof(Message)); }
        }

        public AboutViewModel()
        {
            Title = "Textual";

            EnlargeText = new Command( StartReadingMessage/*() => Device.OpenUri(new Uri("https://xamarin.com/platform"))*/);
        }
        private void StartReadingMessage()
        {
            if (BtnMessage == "Waiting for message...")
                BtnMessage = "Recieve Text";
            else 
                BtnMessage = "Waiting for message...";
        }
        public ICommand EnlargeText { get; }
    }
}