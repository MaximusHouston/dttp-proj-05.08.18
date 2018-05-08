using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DPO.Web.Areas.Apps.Models
{
    public class TradeShowContext : DbContext
    {

        public TradeShowContext() : base("name=TradeShowContext") 
        {
            Database.SetInitializer<TradeShowContext>(null);

            //Database.SetInitializer<TradeShowContext>(new DropCreateDatabaseIfModelChanges<TradeShowContext>());
        }
        
        public DbSet<Requester> Requesters { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<RentingItem> RentingItems { get; set; }
    }
}