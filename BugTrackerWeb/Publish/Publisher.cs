using BugTrackerWeb.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackerWeb.Publish
{
    public class Publisher
    {
        const string ServiceBusConnectionString = "";
        const string QueueName = "bugtracker-queue";
        private IQueueClient queueClient;

        public Publisher()
        {
            this.queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public async Task SendMessage(BugViewModel model)
        {
            try
            {
                var body = JsonConvert.SerializeObject(model);
                var message = new Message(Encoding.UTF8.GetBytes(body));
                await this.queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
