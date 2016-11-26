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
                if (length < 4)
                    return new HttpResponseMessage(HttpStatusCode.OK);

                var luisSays = await CallLuis(activity.Text);

                if(luisSays != null && luisSays.Score > 0.5)
                {
                    var resultPage = await GetResearch(luisSays.Entity);
                    var reply = activity.CreateReply(resultPage);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        public async Task<EntityItem> CallLuis(string query)
        {
            var result = new EntityItem();

            HttpClient client = new HttpClient();
            var baseUri = "https://api.projectoxford.ai/luis/v2.0/apps/96a1fe5f-1ca3-45b0-9ce5-e900a742b052?subscription-key=911ebdef232a46b7a15c01b41957410c&q=";
            var requestUri = baseUri + query;
            var luisResponse = await client.GetStringAsync(requestUri);

            var luisSays = JsonConvert.DeserializeObject<LuisResponse>(luisResponse);

            if(luisSays.TopScoringIntent?.Intent.ToLower() == "research")
            {
                var topic = string.Empty;

                var entities = luisSays.Entities.Where(e => e.Type == "topic")?.OrderByDescending(o => o.Score);

                if (entities == null)
                    return new EntityItem();

                result = entities.FirstOrDefault();
            }
            return result;
        }

        public async Task<string> GetResearch(string topic)
        {
            var resultPage = string.Empty;
            HttpClient client = new HttpClient();
            var baseUri = "https://ortus.rtu.lv";
            var requestUri = baseUri + "/science/lv/publications/search/" + topic;
            var result = await client.GetStringAsync(requestUri);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(result);

            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0 && htmlDoc.DocumentNode == null)
            {
                return "Oops, couldn't find anything";
            }

            var resultsToReturn = string.Empty;
            var resultsTable = htmlDoc.GetElementbyId("resultTable");
            var resultsTableBody = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]/tbody[1]");

            if (resultsTableBody == null)
            {
                return "Oops, nothing found";
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

                resultsToReturn += index.ToString() + ". " + caption + " : " + baseUri + link + "\n\n";
            }

            if(childs.Count() > 5)
            {
                resultsToReturn += "\n\nAnd there is more: " + requestUri;
            }

            resultPage = resultsToReturn;
            return resultsToReturn;
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