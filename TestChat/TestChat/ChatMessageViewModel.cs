using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TestChat
{
    public class ChatMessageViewModel
    {
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();
    }
}
