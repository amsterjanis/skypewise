using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeWise.Models
{
    public class IntentEntityPair
    {
        public IntentEntityPair(IntentItem intent, EntityItem entity)
        {
            this.Intent = intent;
            this.Entity = entity;
        }

        public long Id { get; set; }

        public IntentItem Intent { get; set; }

        public EntityItem Entity { get; set; }

        public string ChannelId { get; set; }
    }
}
