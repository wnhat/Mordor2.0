using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutInspect.ViewModel
{
    public class MessageDialogViewModel:ViewModelBase
    {
        private string message = "DefaultMessage";
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
        public MessageDialogViewModel(string s)
        {
            Message = s;
        }
    }
}
