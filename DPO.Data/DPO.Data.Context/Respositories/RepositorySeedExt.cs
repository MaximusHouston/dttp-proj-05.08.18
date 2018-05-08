using DPO.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
    public partial class Repository
    {
        public IQueryable<Seed> Seeds
        {
            get { return this.GetDbSet<Seed>(); }
        }

        public int GetConfigOrderNumber() {
            int configOrderNumber = 0;

            //get & update seed number right away;
            using (Repository repo = new Repository())
            {
                var Seed = (from seed in repo.Context.Seeds
                            where seed.SeedTypeId == (int)SeedTypeEnum.ConfiguredOrderNumber
                            select seed).FirstOrDefault();
                if (Seed != null)
                {
                    configOrderNumber = Seed.SeedNumber;
                }

                //get & update seed number right away;
                Seed.SeedNumber += Seed.Step;
                repo.Context.Entry(Seed).State = EntityState.Modified;
                repo.SaveChanges();

            }

            return configOrderNumber;
        }

        public int GetSeedNumber(int seedTypeId)
        {
            int configOrderNumber = 0;
            var Seed = (from seed in this.Context.Seeds
                        where seed.SeedTypeId == seedTypeId
                        select seed).FirstOrDefault();
            if (Seed != null)
            {
                configOrderNumber = Seed.SeedNumber;
            }

            return configOrderNumber;
        }

        public void UpdateSeedNumber(int seedTypeId) {
            var entity = (from seed in this.Context.Seeds
                         where seed.SeedTypeId == seedTypeId
                         select seed).FirstOrDefault();
            entity.SeedNumber = entity.SeedNumber + entity.Step;

            this.Context.Entry(entity).State = EntityState.Modified;
            //this.SaveChanges();

        }
    }
}
