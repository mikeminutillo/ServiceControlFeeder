using System;

namespace ConsoleApplication1
{
    public class SentMessage
    {
        public SentMessage()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        public string MessageId { get; }
        public string MessageType { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Intent { get; set; }
        public string RelatedTo { get; set; }
    }
}