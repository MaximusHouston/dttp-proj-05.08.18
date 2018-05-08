//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================


namespace DPO.Common
{
    public class UserLoginModel
    {
        // User info
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Persistent { get; set; }
        public DropDownModel Links { get; set; }
        public string SelectedLink { get; set; }
    }
}
