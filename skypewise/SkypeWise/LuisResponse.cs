using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeWise
{
    public class LuisResponse
    {
        public string Query { get; set; }
        public IntentItem TopScoringIntent { get; set; }
        public List<EntityItem> Entities { get; set; }
    }

    public class IntentItem
    {
        public string Intent { get; set; }
        public double Score { get; set; }
    }
    
    public class EntityItem
    {
        public string Entity { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public double Score { get; set; }
    }
}
