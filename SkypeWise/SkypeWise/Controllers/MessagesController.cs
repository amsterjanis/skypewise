using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using SkypeWise.Helpers;
using HtmlAgilityPack;

namespace SkypeWise
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        string resultPage = string.Empty;
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
                    var searchTerm = activity.Text.Remove(0, "research ".Count());

                    var reply = activity.CreateReply("I'll be back with result...");
                    await connector.Conversations.ReplyToActivityAsync(reply);

                    reply = activity.CreateReply("Wait...");

                    GetResearch(searchTerm);
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
            var baseUrl = "https://ortus.rtu.lv";
            var requestUrl = baseUrl + "/science/lv/publications/search/" + topic;
            var result = await client.GetStringAsync(requestUrl);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(result);

            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0 && htmlDoc.DocumentNode == null)
            {
                resultPage = "Oops, couldn't find anything";
                return;
            }

            var resultsToReturn = string.Empty;
            var resultsTable = htmlDoc.GetElementbyId("resultTable");
            var resultsTableBody = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]/tbody[1]");

            if (resultsTableBody == null)
            {
                resultPage = "Oops, nothing found";
                return;
            }

            var childs = resultsTableBody.ChildNodes.Where(o => !string.IsNullOrEmpty(o.InnerHtml) && o.InnerHtml != "\n");

            var childsToServe = childs.Take(5).ToList();

            var index = 0;
            foreach (var node in childsToServe)
            {
                index++;
                var inspectingNode = node.SelectSingleNode("td[2]/a[1]");
                var caption = inspectingNode.InnerText;
                var link = inspectingNode.GetAttributeValue("href", "/");

                resultsToReturn += index.ToString() + ". " + caption + " : " + baseUrl + link + "\n\n";
            }

            if(childs.Count() > 5)
            {
                resultsToReturn += "\n\nAnd there is more: " + requestUrl;
            }

            resultPage = resultsToReturn;
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