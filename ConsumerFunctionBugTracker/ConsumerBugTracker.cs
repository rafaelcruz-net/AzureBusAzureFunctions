using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using Dapper;

namespace ConsumerFunctionBugTracker
{
    public static class ConsumerBugTracker
    {
        const string ConnectionString = "";

        [FunctionName("ConsumerBugTracker")]
        public static void Run([ServiceBusTrigger("<queuename>", AccessRights.Manage)]BrokeredMessage message, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {message.MessageId}");

            try
            {
                if (message != null)
                {
                    var stream = message.GetBody<Stream>();
                    var reader = new StreamReader(stream);
                    var bug = JsonConvert.DeserializeObject<BugModel>(reader.ReadToEnd());

                    CreateNewBug(bug);

                    message.Complete();
                }
            }
            catch (System.Exception ex)
            {
                message.Abandon();
                log.Info($"Exception occured {ex}");
                throw;
            }
        }

        private static void CreateNewBug(BugModel model)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Execute("INSERT INTO BUG([Title], [Description], [User], [Severity]) VALUES (@Title, @Description, @User, @Severity)", model);
            }
        }

        #region Inner Class
        public class BugModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string User { get; set; }
            public Severity Severity { get; set; }
        }

        public enum Severity
        {
            Block = 1,
            Critical = 2,
            Low = 3
        }

        #endregion

    }
}
