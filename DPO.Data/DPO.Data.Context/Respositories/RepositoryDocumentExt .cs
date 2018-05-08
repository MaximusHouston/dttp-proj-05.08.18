//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;
using System.Reflection;
using System.Transactions;
using System.Threading.Tasks;

namespace DPO.Data
{

    public partial class Repository
    {

        public IQueryable<Document> Documents
        {
            get { return this.GetDbSet<Document>(); }
        }

        public IQueryable<DocumentProductLink> DocumentProductLinks
        {
            get { return this.GetDbSet<DocumentProductLink>(); }
        }

        public IQueryable<Document> DocumentsByProductId(long productId)
        {
            var result = from l in this.Context.DocumentProductLinks
                         where productId == l.ProductId
                         select l.Document;

            return result;
        }

        public IQueryable<Document> GetAllDocuments()
        {
            var result = from doc in this.Context.Documents
                         select doc;

            return result;
        }

        public IQueryable<Document> GetDocumentsQueryBySearch(SearchDocument search)
        {
            IQueryable<Document> query = GetAllDocuments();

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            //query = Sort(query, search);

            return query;
        }

        private IQueryable<Document> Filter(IQueryable<Document> query, SearchDocument search) {
            if (search == null) return query;
           

            if (search.DocumentTypeId.HasValue)
            {
                query = query.Where(s => s.DocumentTypeId == search.DocumentTypeId);
            }

            if (search.PIMUploadDate > new DateTime())
            {
                query = query.Where(s => s.PIMUploadDate >= search.PIMUploadDate);
            }

            return query;
        }



    }
}