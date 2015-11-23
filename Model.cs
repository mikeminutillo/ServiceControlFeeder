using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Model
    {
        public IList<Conversation> Conversations { get; }

        public Model()
        {
            Conversations = new List<Conversation>();
        } 
    }
}