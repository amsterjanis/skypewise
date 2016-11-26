﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace SkypeWise.Services
{
    public class Luis
    {
        public async Task<EntityItem> CallLuis(string query)
        {
            var result = new EntityItem();

            HttpClient client = new HttpClient();
            var baseUri = "https://api.projectoxford.ai/luis/v2.0/apps/96a1fe5f-1ca3-45b0-9ce5-e900a742b052?subscription-key=911ebdef232a46b7a15c01b41957410c&q=";
            var requestUri = baseUri + query;
            var luisResponse = await client.GetStringAsync(requestUri);

            var luisSays = JsonConvert.DeserializeObject<LuisResponse>(luisResponse);

            if (luisSays.TopScoringIntent?.Intent.ToLower() == "research")
            {
                var topic = string.Empty;

                var entities = luisSays.Entities.Where(e => e.Type == "topic")?.OrderByDescending(o => o.Score);

                if (entities == null)
                    return new EntityItem();

                result = entities.FirstOrDefault();
            }
            else
            {
                var entities = luisSays.Entities.Where(e => e.Type == "topic")?.OrderByDescending(o => o.Score);

                if (entities == null)
                    return new EntityItem();

                // TODO here
            }
            return result;
        }
    }
}
