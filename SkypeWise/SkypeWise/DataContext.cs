using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SkypeWise
{
    public class DataContext : DbContext
    {
        public DataContext() 
            : base("DefaultConnection")
        {
        }

        public DbSet<IntentEntityPair> IntentEntityPairs { get; set; }
    }

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