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
                return null;
            }

            var resultsToReturn = string.Empty;
            var resultsTable = htmlDoc.GetElementbyId("photo_grid");
            var resultsTableBody = htmlDoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[1]/div[1]/a[1]/img[1]/@src[1]");

            if (resultsTableBody == null)
            {
                return null;
            }

            foreach(var atr in resultsTableBody.Attributes)
            {
                if (atr.Name == "src")
                    return atr.Value;
            }
            return string.Empty;
        }
    }
}