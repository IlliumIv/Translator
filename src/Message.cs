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
        public string TimeStamp { get; private set; }
        public string Sender { get; private set; }
        public string Text { get; set; }

        public Message(string time, string sender, string text)
        {
            TimeStamp = time;
            Sender = sender;
            Text = text;
        }
    }
}
