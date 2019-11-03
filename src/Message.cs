// Copyright 2019 IlliumIv
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorHelper
{
    public class Message
    {
        public MessageStatus Status { get; set; }
        public string TimeStamp { get; private set; }
        public string Sender { get; private set; }
        public string Text { get; set; }

        public Message(string time, string sender, string text)
        {
            Status = MessageStatus.Original;
            TimeStamp = time;
            Sender = sender;
            Text = text;
        }
    }

    [Flags]
    public enum MessageStatus
    {
        Original = 0,
        Translated = 1,
        Displayed = 2
    }
}
