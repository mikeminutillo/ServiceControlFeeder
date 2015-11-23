using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Conversation
    {
        public string Name { get; }
        public IList<SentMessage> Messages { get; }

        public Conversation(string name)
        {
            Name = name;
            Messages = new List<SentMessage>();
        }
    }
}