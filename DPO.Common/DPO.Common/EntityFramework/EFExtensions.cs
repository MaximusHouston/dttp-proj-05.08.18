//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace DPO.Common
{
   public static class EFExtensions
   {

      static public DbDataRecord OriginalValues(this ObjectContext objectSet, EntityObject enitity)
      {
         return objectSet.ObjectStateManager.GetObjectStateEntry(enitity).OriginalValues;
      }

      static public bool HasProperty(this DbEntityEntry ose, string propertyName)
      {
         return (ose.OriginalValues[propertyName] != null);
      }

      static public bool HasChanged(this DbEntityEntry ose, string propertyName)
      {
          try
          {
              if (ose.State == EntityState.Modified)
              {
                  object orignal = ose.OriginalValues[propertyName];

                  object value = ose.CurrentValues[propertyName];

                  // orignal could be null
                  if ((orignal == null && value != null) || (orignal != null && !orignal.Equals(value)))
                  {
                      return true;
                  }
              }

              if (ose.State == EntityState.Added || ose.State == EntityState.Deleted)
              {
                  return true;
              }
          }
          catch // if fails not found
          {
          }
         return false;
      }

      static public T PreviousValue<T>(this DbEntityEntry ose, string propertyName)
      {
         if (ose.State == EntityState.Modified || ose.State == EntityState.Deleted)
         {
            return (T)ose.OriginalValues[propertyName];
         }

         return (T)ose.CurrentValues[propertyName];
      }

      //public static IQueryable<T> WhereLike<T>(this IQueryable<T> source,Expression<Func<T, string>> valueSelector,string value)
      //{
      //   return source.Where(BuildLikeExpression(valueSelector, value));
      //}

      //public static Expression<Func<T, bool>> BuildLikeExpression<T>(Expression<Func<T, string>> valueSelector,string value)
      //{
      //   if (valueSelector == null)
      //      throw new ArgumentNullException("valueSelector");

      //   var method = GetLikeMethod(value);

      //   value = value.Replace("*","");

      //   var body = Expression.Call(valueSelector.Body, method, Expression.Constant(value));

      //   var parameter = valueSelector.Parameters.Single();

      //   return Expression.Lambda<Func<T, bool>>(body, parameter);
      //}

      //private static MethodInfo GetLikeMethod(string value)
      //{
      //   var methodName = "";

      //   if (value.EndsWith("*") && !value.StartsWith("*"))
      //   {
      //       methodName = "StartsWith";
      //   }
      //   else
      //   if (value.StartsWith("*") && !value.EndsWith("*"))
      //   {
      //       methodName = "EndsWith";
      //   }
      //   else
      //   {
      //      methodName ="Contains";
      //   }

      //   var stringType = typeof(string);

      //   return stringType.GetMethod(methodName, new Type[] { stringType });
      //}

        public static EntityOp<TEntity> Upsert<TEntity>(this DbContext context) where TEntity : class
        {
            return new UpsertOp<TEntity>(context);
        }
      

      public abstract class EntityOp<TEntity, TRet>
      {
          public readonly DbContext Context;
          public readonly TEntity Entity;
          public readonly string TableName;

          public List<Utilities.FastGetProperty<TEntity>> Properties { get; set; }

          private readonly List<string> keyNames = new List<string>();
          public IEnumerable<string> KeyNames { get { return keyNames; } }

          private readonly List<string> excludeProperties = new List<string>();

          private static string GetMemberName<T>(Expression<Func<TEntity, T>> selectMemberLambda)
          {
              var member = selectMemberLambda.Body as MemberExpression;
              if (member == null)
              {
                  throw new ArgumentException("The parameter selectMemberLambda must be a member accessing labda such as x => x.Id", "selectMemberLambda");
              }
              return member.Member.Name;
          }

          public EntityOp(DbContext context)
          {
              Context = context;

              object[] mappingAttrs = typeof(TEntity).GetCustomAttributes(typeof(TableAttribute), false);
              TableAttribute tableAttr = null;
              if (mappingAttrs.Length > 0)
              {
                  tableAttr = mappingAttrs[0] as TableAttribute;
              }

              if (tableAttr == null)
              {
                  throw new ArgumentException("TEntity is missing TableAttribute", "entity");
              }

              TableName = tableAttr.Name;
              this.Properties = new List<Utilities.FastGetProperty<TEntity>>();

              foreach(var prop in typeof(TEntity).GetProperties().Where(pr => !excludeProperties.Contains(pr.Name)).ToList())
              {
                 Properties.Add(Utilities.FastGetPropertySetup<TEntity>(prop.Name)); 
              }
          }

          public abstract TRet Execute(TEntity entity,bool insert, bool delete, bool update);
          public void Run(TEntity entity,bool insert, bool delete, bool update)
          {
              Execute(entity, insert, delete, update);
          }

          public EntityOp<TEntity, TRet> Key<TKey>(Expression<Func<TEntity, TKey>> selectKey)
          {
              keyNames.Add(GetMemberName(selectKey));
              return this;
          }

          public EntityOp<TEntity, TRet> ExcludeField<TField>(Expression<Func<TEntity, TField>> selectField)
          {
              excludeProperties.Add(GetMemberName(selectField));
              return this;
          }

          public List<Utilities.FastGetProperty<TEntity>> ColumnProperties
          {
              get
              {
                  return Properties;
              }
          }
      }

      public abstract class EntityOp<TEntity> : EntityOp<TEntity, int>
      {
          public EntityOp(DbContext context) : base(context) { }

          public sealed override int Execute(TEntity entity, bool insert,bool delete,bool update)
          {
              ExecuteNoRet(entity, insert, delete, update);
              return 0;
          }

          protected abstract void ExecuteNoRet(TEntity entity, bool insert, bool delete, bool update);
      }

      public class UpsertOp<TEntity> : EntityOp<TEntity>
      {
          public UpsertOp(DbContext context) : base(context) { }

          protected override void ExecuteNoRet(TEntity entity, bool insert, bool delete, bool update)
          {
              if (!insert && !delete && !update) return;

              StringBuilder sql = new StringBuilder();

              int notNullFields = 0;
              var valueKeyList = new List<string>();
              var columnList = new List<string>();
              var valueList = new List<object>();
              foreach (var p in ColumnProperties)
              {
                  columnList.Add(p.PropertyName);

                  var val = p.GetValue(entity);
                  if (val != null)
                  {
                      valueKeyList.Add("{" + (notNullFields++) + "}");
                      valueList.Add(val);
                  }
                  else
                  {
                      valueKeyList.Add("null");
                  }
              }
              var columns = columnList.ToArray();

              sql.Append("merge into ");
              sql.Append(TableName);
              sql.Append(" as T ");

              sql.Append("using (values (");
              sql.Append(string.Join(",", valueKeyList.ToArray()));
              sql.Append(")) as S (");
              sql.Append(string.Join(",", columns));
              sql.Append(") ");

              sql.Append("on (");
              var mergeCond = string.Join(" and ", KeyNames.Select(kn => "T." + kn + "=S." + kn));
              sql.Append(mergeCond);
              sql.Append(") ");
              if (update)
              {
                  sql.Append("when matched then update set ");
                  sql.Append(string.Join(",", columns.Select(c => "T." + c + "=S." + c).ToArray()));
              }
              if (insert)
              {
                  sql.Append(" when not matched by S then insert (");
                  sql.Append(string.Join(",", columns));
                  sql.Append(") values (S.");
                  sql.Append(string.Join(",S.", columns));
                  sql.Append(");");
              }
              if (delete)
              {
                  sql.Append(" when not matched by S then DELETE ");
                  sql.Append(string.Join(",", columns));
                  sql.Append(") values (S.");
                  sql.Append(string.Join(",S.", columns));
                  sql.Append(");");
              }

              Context.Database.ExecuteSqlCommand(sql.ToString(), valueList.ToArray());
          }
      }

   }

}

