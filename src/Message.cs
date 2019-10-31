using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorHelper
{
    public class Message
    {
        public DateTime TimeStamp { get; private set; }
        public string Sender { get; private set; }
        public string Text { get; private set; }

        public Message(DateTime time, string sender, string text)
        {
            TimeStamp = time;
            Sender = sender;
            Text = text;
        }
    }
}
