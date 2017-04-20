using Gen.EntityFramework.Entitities.TleEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen.EntityFramework
{
    public class GenDbContext : DbContext
    {
        public GenDbContext() : base("Default") { }
        public GenDbContext(string name) : base(name) { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public IDbSet<Test_Specification> Test_Specification { get; set; }
        public IDbSet<CI_Area_Map> CI_Area_Map { get; set; }
        public IDbSet<CI_Tree> CI_Tree { get; set; }
        public IDbSet<Area_Pack> Area_Pack { get; set; }
        public IDbSet<II_Class> II_Class { get; set; }
        public IDbSet<Job> Job { get; set; }
        public IDbSet<HRLevel> HRLevel { get; set; }
        public IDbSet<Team> Team { get; set; }
        public IDbSet<Personal_Profile> Personal_Profile { get; set; }
        public IDbSet<Person_Job_Map> Person_Job_Map { get; set; }
        public IDbSet<Personal_CI> Personal_CI { get; set; }
        public IDbSet<II_CI_Area_Grade_Mapping> II_CI_Area_Grade_Mapping { get; set; }
        public IDbSet<II_Instance> II_Instance { get; set; }
        public IDbSet<Job_ReqOnAreaPack> Job_ReqOnAreaPack { get; set; }
    }
}
