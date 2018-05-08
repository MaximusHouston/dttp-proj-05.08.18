//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Common.DaikinUniversity;
using DPO.Domain.DaikinUniversity;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public partial class TestDaikinUniversityApiServices : TestAdmin
    {
        private UserSessionModel model = new UserSessionModel();

        private DaikinUniversityApiServices service;

        public TestDaikinUniversityApiServices()
        {
            service = new DaikinUniversityApiServices(this.TContext);
        }

        [Test]
        public void TestDaikinUniversityApiServices_AuthenticateApi()
        {
            var resp = service.AuthenticateApi();
        }

        [Test]
        public void TestDaikinUniversityApiServices_SearchCatalog()
        {
            //SearchCatalog search = new SearchCatalog()
            //{
            //    FromDate = DateTime.Now,
            //    OuId = new List<string> { "DaikinCity" },
            //    OuType = new List<string> { "Division" },
            //    Format = "json",
            //    TrainingType = new List<TrainingType> { TrainingType.Course, TrainingType.SocialLearningProgram }
            //};

            var search = new SearchGlobalCatalog()
            {
                OuId = new List<string> { "DKN_EXT_DKN_DCY" },
                OuType = new List<string> { "Division" },
                Format = "json"
            };

            string queryString = DaikinUniversityUtilities.ConvertObjectToQueryString(search);
            Assert.IsNotNull(queryString);
            Assert.IsNotEmpty(queryString);

            var resp = service.SearchCatalog(search);

            Assert.IsNotNull(resp);
            Assert.IsTrue(resp.IsOK);
            Assert.IsNotNull(resp.Model);

            var model = resp.Model as DaikinUniversityApiResponse<GlobalSearchTrainingItem>;
            Assert.IsNotNull(model);
            Assert.IsNull(model.Error);
        }
    }
}