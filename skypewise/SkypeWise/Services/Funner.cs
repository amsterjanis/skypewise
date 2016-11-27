using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SkypeWise.Services
{
    public class Funner
    {
        public async Task<string> GetFun(string topic)
        {
            var resultPage = string.Empty;
            HttpClient client = new HttpClient();
            var baseUri = "https://pixabay.com";
            var requestUri = baseUri + "/en/photos/?q=" + topic;
            var result = await client.GetStringAsync(requestUri);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(result);

            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0 && htmlDoc.DocumentNode == null)
            {
                return "Oops, couldn't find anything";
            }

            var resultsToReturn = string.Empty;
            var resultsTable = htmlDoc.GetElementbyId("photo_grid");
            var resultsTableBody = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[1]/div[1]/a[1]/img[1]/@src[1]");

            if (resultsTableBody == null)
            {
                return "Oops, nothing found";
            }

            foreach(var atr in resultsTableBody.Attributes)
            {
                if (atr.Name == "src")
                    return atr.Value;
            }
            return string.Empty;

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