using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TestChat
{
    public class ChatMessageViewModel
    {
        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
    }
}
