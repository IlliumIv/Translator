
namespace TranslatorHelper.Message
{
    public class IMessage
    {
        public string TimeStamp { get; private set; }
        public ChatType ChatType { get; private set; }
        public string Sender { get; private set; }
        public string Text { get; set; }

        public IMessage(string time, ChatType chatType, string sender, string text)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            TimeStamp = time.Replace("{", "").Replace("}", "");
            TimeStamp = StringHelper.Replacer.Clean(time);
            ChatType = chatType;
            Sender = sender.Replace("{", "").Replace("}", "");
            Text = text.Replace("{", "").Replace("}", "");
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
