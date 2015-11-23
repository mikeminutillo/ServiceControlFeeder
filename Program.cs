using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Transports;
using NServiceBus.Unicast;

namespace ConsoleApplication1
{
    static class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText(args.First());
            var segments = Segment.Parse(text);

            var model = new Model();

            (new ModelSegmentWalker(model) as ISegmentWalker).Walk(segments);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(model);
            var container = builder.Build();

            var config = new BusConfiguration();
            config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            using (var bus = Bus.CreateSendOnly(config))
            {
                var messageSender = container.Resolve<ISendMessages>();

                foreach (var conversation in model.Conversations)
                {
                    Console.WriteLine(conversation.Name);
                    var conversationId = Guid.NewGuid();

                    foreach (var m in conversation.Messages)
                    {
                        var tm = new TransportMessage();
                        tm.Headers[Headers.MessageId] = m.MessageId;
                        tm.Headers[Headers.ConversationId] = conversationId.ToString();
                        tm.Headers[Headers.EnclosedMessageTypes] = m.MessageType;
                        tm.Headers[Headers.RelatedTo] = m.RelatedTo;
                        tm.Headers[Headers.MessageIntent] = m.Intent;
                        tm.Headers[Headers.OriginatingEndpoint] = m.Sender;
                        tm.Headers[Headers.OriginatingMachine] = "MACHINE";
                        tm.Headers[Headers.TimeSent] = ToWireFormattedString(DateTime.Now);

                        tm.Headers[Headers.ProcessingEndpoint] = m.Receiver;
                        tm.Headers[Headers.ProcessingMachine] = "MACHINE";
                        tm.Headers[Headers.ProcessingStarted] = ToWireFormattedString(DateTime.Now);
                        tm.Headers[Headers.ProcessingEnded] = ToWireFormattedString(DateTime.Now);

                        if (m.Intent == "Timeout")
                        {
                            tm.Headers[Headers.MessageIntent] = "Send";
                            tm.Headers[Headers.IsDeferredMessage] = "true";
                            tm.Headers[Headers.IsSagaTimeoutMessage] = "true";
                        }

                        Console.WriteLine($"\t {m.Intent} {m.MessageType} from {m.Sender} to {m.Receiver}");
                        messageSender.Send(tm, new SendOptions(Address.Parse("audit")));
                    }
                }
            }
        }

        const string Format = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        private static string ToWireFormattedString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime()
                .ToString(Format, CultureInfo.InvariantCulture);
        }
    }
}
