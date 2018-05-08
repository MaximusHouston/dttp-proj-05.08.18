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

namespace DPO.Data
{

    /* 
         
         USSA0 System Admin
               QTRM1 Regional Manager 1
                      QTSE1 Daikin SE 1
                      QTSE2 Daikin SE 2
               QTRM2 Regional Manager 2
                      QTSE3 Daikin SE 3
                            QTCU1 Customer 1
                                    QTCSE1 Customer Sales Engineer - QTB1DEPT1
                                           QTPJ1 - Project 1
                                                       QTP1Q1 
                                                       QTP1Q2 - Active
                                           QTPJ2 - Project 2
     *                                                 QTP2Q3 
     *                                                 QTP2Q4 - Active
                                    QTCSE2 Customer Sales Engineer - QTB1DEPT1
                                           QTPJ3 - Project 3 
     *                                                 QTP3Q5 - Active
                                           QTPJ4 - Project 4
     *                                                 QTP3Q6 - Active
                                    QTCSE3 Customer Sales Engineer - QTB1DEPT2
                                           QTPJ5 - Project 5 
     *                                                 QTP5Q7 - Active
                                           QTPJ6 - Project 6
     *                                                 QTP6Q8 - Active
     *                                                 QTP6Q9 
                            QTCU2 Customer 2
                      QTSE4 Daikin SE 4
                            QTCU3 Customer 3
                                    QTCSE4 Customer Sales Engineer  - QTB1DEPT3
                                           QTPJ7 - Project 7- Active
     *                                                 QTP7Q10
                                           QTPJ8    - Project 8- Active
     *                                                 QTP8Q11
                                    QTCSE5 Customer Sales Engineer  - QTB1DEPT4
                                           QTPJ9 - Project 9 - Active
     *                                                 QTP9Q12
                                           QTPJ10 - Project 10 - Active
     *                                                  QTP10Q13
               QTRM3 Regional Manager 3
                      QTSE5 Daikin SE 5
                            QTCU4 Customer 4

     */

    public partial class SeedFactory 
   {

      public void SeedTestDataProjects()
      {
        var ACM1 = db.AccountMultiplierCreate("ABC", QTB1DEPT1); ACM1.Multiplier = 1.5M;
        var ACM2 = db.AccountMultiplierCreate("ABC", QTB1DEPT2); ; ACM2.Multiplier = 2.5M;
        var ACM3 = db.AccountMultiplierCreate("ABC", QTB1DEPT3); ; ACM3.Multiplier = 1M;
        var ACM4 = db.AccountMultiplierCreate("ABC", QTB1DEPT4); ; ACM4.Multiplier = 0.5M;

         var productFamilies = db.ProductFamilies.ToList();
         var brands = db.Brands.ToList();
         var projecttypes = db.ProjectTypes.ToList();
         var projectstatus = db.ProjectStatusTypes.ToList();
         var bidstatus = db.BidStatusTypes.ToList();
         var invformtype = db.InventoryFromTypes.ToList();

         var products = new List<Product>();
         for(var p=1; p < 25; p++)
         {
            var prod = db.ProductCreate(db.Context.GenerateNextLongId());
            prod.DaikinProductCode = string.Format("Prod {0}", p);
            prod.ProductFamily = productFamilies[p % productFamilies.Count];
            prod.Brand = brands[p % brands.Count];
            prod.ProductClassCode = "ABC"; // for account modifiers
            prod.Price = p;
            db.Context.Products.Add(prod);
            products.Add(prod);
         }


         var QTPJ1 = CreateProject(context, QTCSE1,QTCU1, "Project 1", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ2 = CreateProject(context, QTCSE1, QTCU3, "Project 2", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ3 = CreateProject(context, QTCSE2, QTCU3, "Project 3", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ4 = CreateProject(context, QTCSE2, QTCU3, "Project 4", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ5 = CreateProject(context, QTCSE3, QTCU3, "Project 5", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ6 = CreateProject(context, QTCSE3, QTCU3, "Project 6", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ7 = CreateProject(context, QTCSE4, QTCU3, "Project 7", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ8 = CreateProject(context, QTCSE4, QTCU3, "Project 8", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ9 = CreateProject(context, QTCSE5, QTCU3, "Project 9", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);
         var QTPJ10 = CreateProject(context, QTCSE5, QTCU3, "Project 10", DateTime.Now, projecttypes[0], projectstatus[0], bidstatus[0], invformtype[0]);

         if (!Db.Projects.Any(u => u.Name == "Project 1"))
         {
             context.SaveChanges();
         }

         var QTP1Q1 = CreateQuote(context, QTPJ1, "Quote 1", PricingMethodTypeEnum.MarkUp, false,1);
         var QTP1Q2 = CreateQuote(context, QTPJ1, "Quote 2", PricingMethodTypeEnum.MarkUp, true, 2);
         var QTP2Q3 = CreateQuote(context, QTPJ2, "Quote 3", PricingMethodTypeEnum.MarkUp, false, 1);
         var QTP2Q4 = CreateQuote(context, QTPJ2, "Quote 4", PricingMethodTypeEnum.MarkUp, true, 2);
         var QTP3Q5 = CreateQuote(context, QTPJ3, "Quote 5", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP4Q6 = CreateQuote(context, QTPJ4, "Quote 6", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP5Q7 = CreateQuote(context, QTPJ5, "Quote 7", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP6Q8 = CreateQuote(context, QTPJ6, "Quote 8", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP6Q9 = CreateQuote(context, QTPJ6, "Quote 9", PricingMethodTypeEnum.MarkUp, false, 1);
         var QTP7Q10 = CreateQuote(context, QTPJ7, "Quote 10", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP8Q11 = CreateQuote(context, QTPJ8, "Quote 11", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP9Q12 = CreateQuote(context, QTPJ9, "Quote 12", PricingMethodTypeEnum.MarkUp, true, 1);
         var QTP10Q13 = CreateQuote(context, QTPJ10, "Quote 13", PricingMethodTypeEnum.MarkUp, true, 1);



         AddProductsToQuote(QTP1Q1, products, 1, ACM1);
         AddProductsToQuote(QTP1Q2, products, 2, ACM1);
         AddProductsToQuote(QTP2Q3, products, 3, ACM1);
         AddProductsToQuote(QTP2Q4, products, 4, ACM1);
         AddProductsToQuote(QTP3Q5, products, 5, ACM2);
         AddProductsToQuote(QTP4Q6, products, 6, ACM2);
         AddProductsToQuote(QTP5Q7, products, 7, ACM2);  
         AddProductsToQuote(QTP6Q8, products, 8, ACM2);  
         AddProductsToQuote(QTP6Q9, products, 9, ACM2);  
         AddProductsToQuote(QTP7Q10, products, 10, ACM3);  
         AddProductsToQuote(QTP8Q11, products, 11, ACM3);  
         AddProductsToQuote(QTP9Q12, products, 12, ACM4);
         AddProductsToQuote(QTP10Q13, products, 13, ACM4);

         if (!Db.Quotes.Any(u => u.Title == "Quote 13"))
         {
             context.SaveChanges();
         }

      }

      // ####################################################################################
      // Add products to a quote
      // ####################################################################################
      private void SeedTestAddProductsToQuote(Quote quote, List<Product> products, int productSelectionVariance, AccountMultiplier multiplier)
      {
        
          for (int i = 0; i < products.Count; i++)
          {
              if (productSelectionVariance % (i+1) == 0)
              {
                  var item = CreateTestQuoteItem(context,quote,products[i],i+1,multiplier);

                  quote.QuoteItems.Add(item);

                  quote.Total = +item.Price * ((item.Multiplier == null) ? 1 : item.Multiplier.Value) * item.Quantity;
              }
          }
      }

   }
}