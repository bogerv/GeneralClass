using Gen.EntityFramework.Entitities.TleEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework.Context
{
    public class TLEContext : DbContext
    {
        public TLEContext() : base("TLE") { }
        public TLEContext(string name) : base(name) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public IDbSet<UserLevel> UserLevels { get; set; }
        public IDbSet<Level> Levels { get; set; }
        public IDbSet<Modality> Modalities { get; set; }
        public IDbSet<UserInfo> Users { get; set; }
        public IDbSet<Product> Products { get; set; }
    }
}
