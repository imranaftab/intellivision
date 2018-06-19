using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InteliVision.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private ICommand _textCommand;

        public ICommand TextCommand
        {
            get
            {
                if(_textCommand == null)
                {
                    _textCommand = new Command(OnTextCommand);
                }
                return _textCommand;
            }
        }

        private void OnTextCommand(object obj)
        {

        }
    }
}
