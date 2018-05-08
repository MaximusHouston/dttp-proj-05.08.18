//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using System.Threading;

namespace DPO.Tests
{

    [TestClass]
    public partial class TestsEmailServices : TestAdmin
    {
        SendEmailModel model = new SendEmailModel();

        EmailServices service = null;

        public TestsEmailServices()
        {
            service = new EmailServices(this.TContext);
        }


        [TestMethod]
        public void TestSendingEmail()
        {
            SendEmailModel model = new SendEmailModel
            {
                BodyHtmlVersion = "Test",
                BodyTextVersion = "Test",
                Subject = "Test Subject"
            };

            model.From = new MailAddress("NoReply@daikincomfort.com");

            model.To = new List<MailAddress>();
            model.To.Add(new MailAddress("Aaron.Nguyen@daikincomfort.com"));
            model.To.Add(new MailAddress("Charles.Teel@daikincomfort.com"));

            this.service.SendEmail(model);

            Thread.Sleep(100000);
        }

    }
}