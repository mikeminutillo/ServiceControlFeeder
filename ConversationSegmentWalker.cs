namespace ConsoleApplication1
{
    public class ConversationSegmentWalker : SegmentWalkerBase<ConversationSegmentWalker>
    {
        private readonly Conversation _conversation;
        private readonly SentMessage _previousMessage;
        private readonly string _startingEndpoint;

        string CurrentEndpoint => _previousMessage?.Receiver ?? _startingEndpoint;

        public ConversationSegmentWalker(Conversation conversation, SentMessage previousMessage = null, string startingEndpoint = null)
        {
            _conversation = conversation;
            _previousMessage = previousMessage;
            _startingEndpoint = startingEndpoint;
        }

        [Parse("> (.*): (.*)")]
        public ConversationSegmentWalker Send(string messageType, string destinationEndpoint)
        {
            var message = AddMessageToConversation("Send", messageType, destinationEndpoint);
            return new ConversationSegmentWalker(_conversation, message);
        }

        private SentMessage AddMessageToConversation(string intent, string messageType, string destinationEndpoint)
        {
            var message = new SentMessage
            {
                Intent = intent,
                MessageType = messageType,
                Sender = CurrentEndpoint,
                Receiver = destinationEndpoint,
                RelatedTo = _previousMessage?.MessageId
            };

            _conversation.Messages.Add(message);
            return message;
        }

        [Parse("~ (.*): (.*)")]
        public ConversationSegmentWalker Publish(string messageType, string subscriberEndpoint)
        {
            var message = AddMessageToConversation("Publish", messageType, subscriberEndpoint);

            return new ConversationSegmentWalker(_conversation, message);
        }

        [Parse("@ (.*)")]
        public ConversationSegmentWalker Timeout(string messageType)
        {
            var message = AddMessageToConversation("Timeout", messageType, CurrentEndpoint);
            return new ConversationSegmentWalker(_conversation, message);
        }

        [Parse("< (.*)")]
        public ConversationSegmentWalker Reply(string messageType)
        {
            var message = AddMessageToConversation("Reply", messageType, _previousMessage?.Sender);

            return new ConversationSegmentWalker(_conversation, message);
        }
    }
}