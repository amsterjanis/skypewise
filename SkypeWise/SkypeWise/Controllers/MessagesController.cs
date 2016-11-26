using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using SkypeWise.Helpers;

namespace SkypeWise
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        string resultPage="";
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                var reg = new Regex(@"transfer (\d{1,3})");
                var regMatch = reg.Match(activity.Text.ToLower());
                if (regMatch.Success)
                {
                    var replyWithConfirmation = activity.CreateReply($"You try to transfer {regMatch.Groups[0]} EUR monies to someone");
                    await connector.Conversations.ReplyToActivityAsync(replyWithConfirmation);
                }
                else if (activity.Text.ToLower().StartsWith("research "))
                {
                    var reply = activity.CreateReply("I'll be back with result...");
                    await connector.Conversations.ReplyToActivityAsync(reply);

                    reply = activity.CreateReply("Wait...");

                    GetResearch("cryptography");
                    while (resultPage == "")
                    {
                        System.Threading.Thread.Sleep(1000);
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    };

                    reply = activity.CreateReply(resultPage);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    //// return our reply to the user
                    Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        public async void GetResearch(string topic)
        {
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("https://ortus.rtu.lv/science/lv/publications/search/" + topic);
            int start = result.IndexOf("resultTable");
            int stop = result.Substring(start).IndexOf("</tbody></table>");

            resultPage = result.Substring(start, stop);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}