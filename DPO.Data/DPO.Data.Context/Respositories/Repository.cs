
using DPO.Common;
using EntityFramework.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using log4net;

namespace DPO.Data
{
   public partial class  Repository : IDisposable
   {
      public DPOContext Context { get; set; }
      private ILog _log = LogManager.GetLogger("Repository");

        /// <summary>
        /// List of entries in the context
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DbEntityEntry> Entries()
      {
        return this.Context.ChangeTracker.Entries(); 
      }


      /// <summary>
      /// Get Entry in the context
      /// </summary>
      /// <param name="entity"></param>
      /// <returns></returns>
      public DbEntityEntry Entry(object entity) 
      {
          return this.Context.Entry(entity); 
      }

      public Repository()
      {
          this.Context = new DPOContext();
          this.Log = _log;
      }
        public ILog Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        public Repository(DPOContext context)
      {
          if (context == null)
          {
              context = new DPOContext();
          }

          this.Context = context;
      }

      public bool ReadOnly
      {
          set
          {
              this.Context.ReadOnly = value;
          }
      }

 

      /// <summary>
      /// Get a reference to an entity set
      /// </summary>
      /// <typeparam name="TEntity"></typeparam>
      /// <returns></returns>
      protected virtual DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
      {
         return this.Context.Set<TEntity>();
      }

      /// <summary>
      /// Query handling paging
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="query"></param>
      /// <param name="paging"></param>
      /// <returns></returns>
      public IQueryable<T> Paging<T>(UserSessionModel user, IQueryable<T> query, ISearch paging)
      {
        if (paging.PageSize == Constants.DEFAULT_PAGESIZE_RETURN_ALL) return query;


        if (user != null) { 
            if (paging.PageSize.Value == Constants.DEFAULT_USER_DISPLAYSETTINGS_PAGESIZE)
            {
                paging.PageSize = user.DisplaySettingsPageSize;
            }

            if (paging.PageSize != user.DisplaySettingsPageSize)
            {
                user.DisplaySettingsPageSize = paging.PageSize.Value;
                this.SaveDisplaySettings(user);
            }
        }

         if (paging.Page.HasValue && paging.Page > 0 && paging.PageSize.HasValue && paging.PageSize > 0)
         {

           if (paging.ReturnTotals && (paging.Page.Value - 1) * paging.PageSize.Value > paging.TotalRecords - 1)
           {
             paging.Page = paging.TotalRecords / paging.PageSize.Value;
           }

           if (paging.Page <= 0)
           {
             paging.Page = 1;
           }

           int start = (paging.Page.Value - 1) * paging.PageSize.Value;

           query = query.Skip(start).Take(paging.PageSize.Value);

           
         }

         return query;
      }

      /// <summary>
      /// Unit of work save
      /// </summary>
      public void SaveChanges()
      {
         this.Context.SaveChanges();
      }

      /// <summary>
      /// Override entity state
      /// </summary>
      /// <param name="entity"></param>
      /// <param name="entityState"></param>
      protected virtual void SetEntityState(object entity, EntityState entityState)
      {
         this.Context.Entry(entity).State = entityState;
      }


      /// <summary>
      /// Finalise release resources
      /// </summary>
      public void Dispose()
      {
         if (this.Context != null)
         {
            this.Context.Dispose();
         }
      }
      public bool IsUpdateOccurring()
      {
          return this.Context.ChangeTracker.Entries().Any(e => e.State != EntityState.Unchanged);
      }

   }
}