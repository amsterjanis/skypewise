using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace SkypeWise.Services
{
    public class Researcher
    {
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
                return null;
            }

            var resultsToReturn = string.Empty;
            var resultsTable = htmlDoc.GetElementbyId("resultTable");
            var resultsTableBody = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]/tbody[1]");

            if (resultsTableBody == null)
            {
                return null;
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

            if (childs.Count() > 5)
            {
                resultsToReturn += "\n\nAnd there is more: " + requestUri;
            }

            resultPage = resultsToReturn;
            return resultsToReturn;
        }
    }
}
