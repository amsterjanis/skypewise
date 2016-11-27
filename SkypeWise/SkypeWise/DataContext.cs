using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SkypeWise.Models;

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
}