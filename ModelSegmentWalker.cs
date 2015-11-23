namespace ConsoleApplication1
{
    public class ModelSegmentWalker : SegmentWalkerBase<ModelSegmentWalker>
    {
        private readonly Model _model;

        public ModelSegmentWalker(Model model)
        {
            _model = model;
        }

        [Parse("(.*): (.*)")]
        public ConversationSegmentWalker CreateConversation(string conversationName, string startingEndpoint)
        {
            var conversation = new Conversation(conversationName);
            _model.Conversations.Add(conversation);

            return new ConversationSegmentWalker(conversation, startingEndpoint: startingEndpoint);
        }
    }
}