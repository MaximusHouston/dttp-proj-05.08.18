using Newtonsoft.Json;

//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    [Serializable]
    public class PageModel : IConcurrency
    {
        public PageModel()
        {
        }

        public long Concurrency { get; set; }

        [JsonIgnore]
        public UserSessionModel CurrentUser { get; set; }

        public int PageNumber { get; set; }
        public int PageTotal { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:G}")]
        public DateTime Timestamp
        {
            get
            {
                return new DateTime(Concurrency);
            }
            set
            {
                Concurrency = value.Ticks;
            }
        }
    }
}