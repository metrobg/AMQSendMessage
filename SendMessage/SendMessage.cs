using System;
using Apache.NMS;


namespace SendMessage
{
    class SendMessage
    {

        static void Main(string[] args)
        {
            String user        = Env("ACTIVEMQ_USER", "admin");
            String password    = Env("ACTIVEMQ_PASSWORD", "Ign32ORw3C4b");
            String host        = Env("ACTIVEMQ_HOST", "192.168.144.172");
            String username    = null;
            String local_host  = null;
            String remote_host = null;
            String in_out      = null;
            bool TEST = true;

            username     = Arg(args, 0, "unknown_user");
            local_host   = Arg(args, 1, "unknown_local");
            remote_host  = Arg(args, 2, "unknown_remote");
            in_out       = Arg(args, 3, "IN");
            if (TEST)
            {
                
                password = "admin";
                host = "192.168.10.66";
            }
            DateTime now = DateTime.Now;

            string text = "User=" + username + ";RemoteHost=" + remote_host + ";ComputerName=" + local_host + ";TimeStamp=" + now + ";in_out=" + in_out.ToUpper();
            SendNewMessage(text, host, password, user);
        }

        private static void SendNewMessage(string text, string host, string password, string user)
        {
            //string topic = "TextQueue";
            string topic = "GPDC-LOGIN";

            // Console.WriteLine($"Adding message to queue topic: {topic}");

            String brokerUri = "activemq:tcp://" + host + ":" + "61616";
            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);
            using (IConnection connection = factory.CreateConnection(user, password))

            {
                connection.Start();

                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                using (IDestination dest = session.GetQueue(topic))
                using (IMessageProducer producer = session.CreateProducer(dest))
                {
                    producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    producer.Send(session.CreateTextMessage(text));
                    Console.WriteLine($"Sending: {text} ");

                    session.Dispose();
                }
                if (connection.IsStarted)
                {
                    connection.Close();
                }

            }

        }
        private static String Env(String key, String defaultValue)
        {
            String rc = System.Environment.GetEnvironmentVariable(key);
            if (rc == null)
            {
                return defaultValue;
            }
            return rc;
        }

        private static String Arg(String[] args, int index, String defaultValue)
        {
            if (index < args.Length)
            {
                return args[index];
            }
            return defaultValue;
        }
    }

}
