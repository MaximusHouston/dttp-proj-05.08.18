using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPO.Web.Areas.Apps.Models;
using System.Net.Mail;
using System.Text;
using DPO.Web.Controllers;

namespace DPO.Web.Areas.Apps.Controllers
{
    [Authorise(NoSecurityRequired=true)]
    
    public class SharingProgramController : BaseController
    {

        TradeShowContext context = new TradeShowContext();

        TradeShowVM tradeShowVM = new TradeShowVM();
        
        public ActionResult Index()
        {
            List<Requester> Requesters = context.Requesters.ToList(); 
            return View(Requesters);
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ActionName("EditTradeShow")]
        public ActionResult Edit(TradeShowVM tradeShowVM)
        {
            
            return View();
        }

        //[Authorize]
        public ActionResult Create()
        {
            LoadTradShowData(tradeShowVM);
            return View(tradeShowVM);
        }

        public void LoadTradShowData(TradeShowVM vm)
        {

            List<RentingItem> rentingItems = (from rt in context.RentingItems
                                              select rt
                                             ).ToList();

            vm.RentingItems = rentingItems;
        }

        [HttpPost]
        public ActionResult Create(TradeShowVM tradeShowVM, bool IsSameShippingAddress = false)
        {
            LoadTradShowData(tradeShowVM);

            if (IsSameShippingAddress)
            {
                tradeShowVM.Shipping.Address = tradeShowVM.Event.Address;
                tradeShowVM.Shipping.City = tradeShowVM.Event.City;
                tradeShowVM.Shipping.State = tradeShowVM.Event.State;
                tradeShowVM.Shipping.Zip = tradeShowVM.Event.ZipCode;

                ModelState.Remove("Shipping.Address");
                ModelState.Remove("Shipping.City");
                ModelState.Remove("Shipping.State");
                ModelState.Remove("Shipping.Zip");
            }

            foreach(var item in tradeShowVM.RentingItems)
            {
                if (tradeShowVM.SelectedRentingItems != null && tradeShowVM.SelectedRentingItems.Contains(item.ID))
                {
                    item.Selected = true;
                    if ( tradeShowVM.Quantity1 > 0 && item.ID == 23)
                    {
                        item.Quantity = tradeShowVM.Quantity1;
                    }
                    if(tradeShowVM.Quantity2 > 0 && item.ID == 24)
                    {
                        item.Quantity = tradeShowVM.Quantity2;
                    }
                    if ( tradeShowVM.Quantity2 > 0 && tradeShowVM.Size > 0 && item.ID == 25)
                    {
                        item.Size = tradeShowVM.Size;
                        item.Quantity = tradeShowVM.Quantity3;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                context.Requesters.Add(tradeShowVM.Requester);
                //context.RentingItems.Add(tradeShowVM.RentingItems);
                context.Events.Add(tradeShowVM.Event);
                context.Shippings.Add(tradeShowVM.Shipping);
                
                context.SaveChanges();

                SendEmail(tradeShowVM);

                ResetRentingItems(tradeShowVM);

                context.SaveChanges();

                return View("Confirmation");
            }
           
            return View(tradeShowVM);
        }

        public void ResetRentingItems(TradeShowVM tradeShowVM)
        {
            foreach ( var item in tradeShowVM.RentingItems)
            {
                if(item.Selected)
                {
                    item.Selected = false;
                }
            }
        }

        [HttpGet]
        [ActionName("DeleteTradeShow")]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(TradeShowVM tradeShowVM)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(TradeShowVM tradeShowVM)
        {
            if (ModelState.IsValid)
            {
                MailMessage mailToManager = new MailMessage();
                MailMessage mailToRequester = new MailMessage();

                mailToManager.To.Add("Kristin.hatteberg@daikincomfort.com,mmann@skylinehouston.com,aaron.nguyen@daikincomfort.com");  
                //mailToManager.To.Add("aaron.nguyen@daikincomfort.com");

                mailToRequester.To.Add(tradeShowVM.Requester.Email);
                
                mailToManager.From = new MailAddress(tradeShowVM.Requester.Email);
                mailToRequester.From = new MailAddress("Kristin.hatteberg@daikincomfort.com");
                
                mailToManager.Subject = "Trade Show Request";
                mailToRequester.Subject = "Confirmation Email from Daikin";

                StringBuilder mailToManagerBody = CreateEmailBodyForManager(tradeShowVM);
                StringBuilder mailToRequesterBody = CreateEmailBodyForRequester(tradeShowVM);

                mailToManager.Body = mailToManagerBody.ToString();
                mailToRequester.Body = mailToRequesterBody.ToString();

                mailToManager.IsBodyHtml = true;
                mailToRequester.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "Mailgw.goodmanmfg.com";
                smtp.Port = 25;
              
                try
                {
                    smtp.Send(mailToManager);
                    smtp.Send(mailToRequester);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                return View("_Confirmation", tradeShowVM);
            }
            else
            {
                return View("_SendMailError");
            }
        }

        public StringBuilder CreateEmailBodyForManager(TradeShowVM tradeShowVM)
        {
            StringBuilder EmailBody = new StringBuilder();

            EmailBody.Append("<h3><font color='#00A1E4'>Requester Information</font></h3>");
            EmailBody.AppendLine();
            EmailBody.Append("<b>Order Number:</b>");
            EmailBody.Append(tradeShowVM.Requester.ID);
            EmailBody.AppendLine();
            EmailBody.Append("<br/>");

            string Requester = "<b>Requester:</b>" + "<br/>" + tradeShowVM.Requester.FirstName + " " + tradeShowVM.Requester.LastName  + "<br/>" + tradeShowVM.Requester.Title;
            EmailBody.Append(Requester);
            EmailBody.Append("<br/>");
           
            string RequesterCompany = "<b>Company :</b>" + tradeShowVM.Requester.Company ;
            EmailBody.Append(RequesterCompany);
            EmailBody.Append("<br/>");

            string RequesterEmail = "<b>Email :</b>" + tradeShowVM.Requester.Email;
            EmailBody.Append(RequesterEmail);
            EmailBody.Append("<br/>");

            string RequesterContactNumber = "<b>Contact Number: </b>" + tradeShowVM.Requester.ContactNumber;
            EmailBody.Append(RequesterContactNumber);
            EmailBody.Append("<br/>");

            EmailBody.Append("<h3><font color='#00A1E4'>Event Information</font></h3>");
 
            string EventName = "<b>Event Name: </b>" + tradeShowVM.Event.EventName;
            string StartDate = "<b>Start Date: </b>" + tradeShowVM.Event.StartDate.ToShortDateString();
            string SetupDate = "<b>Setup Date: </b>" + tradeShowVM.Event.SetupDate.ToShortDateString();
            string EndDate = "<b>End Date: </b>" + tradeShowVM.Event.EndDate.ToShortDateString();
            string BoothSize = "<b>Booth Size: </b>" + tradeShowVM.Event.BoothSize.ToString();
            string Location = "<b>Event Location: </b>" + tradeShowVM.Event.Location;
            string EventAddress = "<b>Event Address: </b>" + tradeShowVM.Event.Address;
            string EventCity = ", " + tradeShowVM.Event.City;
            string EventState = ", " + tradeShowVM.Event.State;
            string EventZipCode = ", " + tradeShowVM.Event.ZipCode;
            string Attendee = "<b>Attendee setting up the Booth: </b>" + tradeShowVM.Event.Attendee;
            string AttendeeContactNumber = "<b>Attendee Contact Number: </b>" + tradeShowVM.Event.AttendeePhone;
            string Comments = "<b>Comments/Special Requests: </b>" + tradeShowVM.Event.Comments;

            EmailBody.Append(EventName);
            EmailBody.Append("<br/>");
            EmailBody.AppendLine(StartDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(SetupDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(EndDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(BoothSize);
            EmailBody.Append("<br/>");
            EmailBody.Append(Location);
            EmailBody.Append("<br/>");
            EmailBody.Append(EventAddress);
            EmailBody.Append( EventCity + "  " + EventState + "  " + EventZipCode);
            EmailBody.Append("<br/>");
            EmailBody.Append(Attendee);
            EmailBody.Append("<br/>");
            EmailBody.Append(AttendeeContactNumber);
            EmailBody.Append("<br/>");
            EmailBody.Append(Comments);
            EmailBody.Append("<br/>");
            EmailBody.Append("<br/>");

            EmailBody.Append("<h3><font color='#00A1E4'>Shipping Information</font></h3>");
           
            string ShippingCompany = "<b>Company : </b>" + tradeShowVM.Shipping.Company;
            string ShippingContactName = "<b>ContactName : </b>" + tradeShowVM.Shipping.ContactName;
            string ShippingAddress = "<b>Shipping Address: </b>" + tradeShowVM.Shipping.Address;
            string ShippingCity =  tradeShowVM.Shipping.City;
            string ShippingState = tradeShowVM.Shipping.State;
            string ShippingZipCode =  tradeShowVM.Shipping.Zip;

            EmailBody.Append(ShippingCompany);
            EmailBody.Append("<br/>");
            EmailBody.Append(ShippingContactName);
            EmailBody.Append("<br/>");
            EmailBody.Append(ShippingAddress);
            EmailBody.Append(", " + ShippingCity + ", " + ShippingState + ", " + ShippingZipCode);

            EmailBody.Append("<br/>");
            EmailBody.Append("<br/>");

            EmailBody.Append("<h3><font color='#00A1E4'>Booth Components</font></h3>");

            int boothCount = 0;

            for (int i = 0; i < 7; i++ )
            {
                if ( tradeShowVM.RentingItems[i].Selected)
                {
                    boothCount += 1;
                }
            }

                if (boothCount > 0)
                {
                    EmailBody.Append("<h4>8FT Booth</h4>");

                    EmailBody.Append("<ul>");

                    for (int i = 0; i < 7; i++)
                    {
                        if (tradeShowVM.RentingItems[i].Selected)
                        {
                            string Name = tradeShowVM.RentingItems[i].Name;
                            EmailBody.Append("<li>");
                            EmailBody.Append(Name);
                            EmailBody.Append("</li>");
                        }
                    }

                    EmailBody.Append("</ul>");
                }


            EmailBody.Append("<h4>Pull-Up Banners</h4>");
            
            int Selected = 0;

            EmailBody.Append("<ul>");

            for (int i = 7; i < 22; i++ )
            {
                if (tradeShowVM.RentingItems[i].Selected)
                {
                    string Name = tradeShowVM.RentingItems[i].Name;
                    EmailBody.Append("<li>");
                    EmailBody.Append(Name);
                    EmailBody.Append("</li>");
                    Selected += 1;
                }
            }

            EmailBody.Append("</ul>");

            if ( Selected == 0 )
            {
                EmailBody.Append("None");
                EmailBody.Append("<br/>");
            }

            for (int i = 22; i <= 24; i++)
            {
                if (tradeShowVM.RentingItems[i].Selected)
                {
                    string Name = "<b>" + tradeShowVM.RentingItems[i].Name + "</b>";
                    string Size = "";
                    string Quantity = "<b>Quantity: </b>";

                    EmailBody.Append(Name);
                    EmailBody.Append("<br/>");

                    if (i == 24)
                    {
                        Size = "<b>Size: " + tradeShowVM.RentingItems[i].Size.ToString() + "</b>";
                        EmailBody.Append(Size);
                        EmailBody.Append("<br/>");
                    }

                    if (tradeShowVM.RentingItems[i].Quantity > 0)
                    {
                        Quantity += tradeShowVM.RentingItems[i].Quantity.ToString();

                    }
                    else
                    {
                        Quantity += "Not Specific";
                    }

                    EmailBody.Append(Quantity);
                    EmailBody.Append("<br/>");
                    EmailBody.Append("<br/>");

                }

            }
            
            EmailBody.Append("<br/>");
            EmailBody.Append("<br/>");

            return EmailBody;
        }

        public StringBuilder CreateEmailBodyForRequester(TradeShowVM tradeShowVM)
        {
            StringBuilder EmailBody = new StringBuilder();

            EmailBody.Append("<br/>");
            EmailBody.Append("Dear ");
            string Requester = tradeShowVM.Requester.FirstName + " " + tradeShowVM.Requester.LastName ;
            EmailBody.Append(Requester);
            EmailBody.Append("<br/>");
            EmailBody.Append("<br/>");

            EmailBody.Append("Thanks for completing the online order form for the trade show sharing program. Your order number is " + "<b>" + tradeShowVM.Requester.ID + "</b>" + 
                             ". Please save this number for future reference.");

            EmailBody.Append("<br/>");
         
            EmailBody.Append("<h4>Here's a recap of your order:</h4>");
 
            string EventName = "<b>Event Name: </b>" + tradeShowVM.Event.EventName;
            string StartDate = "<b>Start Date: </b>" + tradeShowVM.Event.StartDate.ToShortDateString();
            string SetupDate = "<b>Setup Date: </b>" + tradeShowVM.Event.SetupDate.ToShortDateString();
            string EndDate = "<b>End Date: </b>" + tradeShowVM.Event.EndDate.ToShortDateString();
            string BoothSize = "<b>Booth Size: </b>" + tradeShowVM.Event.BoothSize.ToString();
            string Location = "<b>Event Location: </b>" + tradeShowVM.Event.Location;
            string EventAddress = "<b>Event Address: </b>" + tradeShowVM.Event.Address;
            string EventCity = ", " + tradeShowVM.Event.City;
            string EventState = ", " + tradeShowVM.Event.State;
            string EventZipCode = ", " + tradeShowVM.Event.ZipCode;
            string Attendee = "<b>Attendee setting up the Booth: </b>" + tradeShowVM.Event.Attendee;
            string AttendeeContactNumber = "<b>Attendee Contact Number: </b>" + tradeShowVM.Event.AttendeePhone;
            string Comments = "<b>Comments/Special Requests: </b><br>" + tradeShowVM.Event.Comments + "</br>";
            EmailBody.Append(EventName);
            EmailBody.Append("<br/>");
            EmailBody.AppendLine(StartDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(SetupDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(EndDate);
            EmailBody.Append("<br/>");
            EmailBody.Append(BoothSize);
            EmailBody.Append("<br/>");
            EmailBody.Append(Location);
            EmailBody.Append("<br/>");
            EmailBody.Append(EventAddress);
            EmailBody.Append(EventCity + "  " + EventState + "  " + EventZipCode);
            EmailBody.Append("<br/>");
            EmailBody.Append(Attendee);
            EmailBody.Append("<br/>");
            EmailBody.Append(AttendeeContactNumber);
            EmailBody.Append("<br/>");
            EmailBody.Append(Comments);
            EmailBody.Append("<br/>");
           
            EmailBody.Append("<h3><font color='#00A1E4'>Booth Components</font></h3>");
            EmailBody.Append("<h4>8FT Booth</h4>");

            EmailBody.Append("<ul>");

            for (int i = 0; i < 7; i++)
            {
                if (tradeShowVM.RentingItems[i].Selected)
                {
                    string Name = tradeShowVM.RentingItems[i].Name;
                    EmailBody.Append("<li>");
                    EmailBody.Append(Name);
                    EmailBody.Append("</li>");
                }
            }

            EmailBody.Append("</ul>");

            EmailBody.Append("<h4>Pull-Up Banners</h4>");

            int Selected = 0;

            EmailBody.Append("<ul>");

            for (int i = 7; i < 22; i++)
            {
                if (tradeShowVM.RentingItems[i].Selected)
                {
                    string Name = tradeShowVM.RentingItems[i].Name;
                    EmailBody.Append("<li>");
                    EmailBody.Append(Name);
                    EmailBody.Append("</li>");
                    Selected += 1;
                }
            }

            EmailBody.Append("</ul>");

            if (Selected == 0)
            {
                EmailBody.Append("None");
                EmailBody.Append("<br/>");
            }

            EmailBody.Append("<br/>");
         
            for (int i = 22; i <= 24; i++)
            {
                if (tradeShowVM.RentingItems[i].Selected)
                {
                    string Name = "<b>" + tradeShowVM.RentingItems[i].Name + "</b>";
                    string Size="";
                    string Quantity = "<b>Quantity: </b>";
                    
                    EmailBody.Append(Name);
                    EmailBody.Append("<br/>");

                    if ( i == 24)
                    {
                        Size = "<b>Size: " + tradeShowVM.RentingItems[i].Size.ToString() + "</b>";
                        EmailBody.Append(Size);
                        EmailBody.Append("<br/>");
                    }
                    
                    if (tradeShowVM.RentingItems[i].Quantity > 0)
                    {
                        Quantity += tradeShowVM.RentingItems[i].Quantity.ToString();

                    }
                    else
                    {
                        Quantity += "Not Specific";
                    }

                    EmailBody.Append(Quantity);
                    EmailBody.Append("<br/>");
                    EmailBody.Append("<br/>");
                }

            }
    
            EmailBody.Append("<h4>What happens now?</h4>");
           
            EmailBody.Append("The items you ordered will be collected, packaged, and then shipped to your event (or the shipping address you provided." +  
                             "If you are a Channel Partner, please remember that you are responsible for the shipping cost. ");
            EmailBody.Append("<br/><br/>");

            EmailBody.Append("Do you need literature (i.e. brochures, products fliers, etc.) for your event? You can order them directly through the HLC " +
                             "by clicking this link http://apps.goodmanmfg.com/brochures/?i=3.");

            EmailBody.Append("<br/><br/>");
            EmailBody.Append("Do you need branded or promotional items for the event? Please order items directly at the Daikin company store by clicking here " +
                             "http://www.daikincompanystore.com/.");

            EmailBody.Append("<br/><br/>");
            EmailBody.Append("<h4>What happens after the event?</h4>");
            
            EmailBody.Append("You are responsible for preparing all the “borrowed” booth components for shipping in the same packaging you received it in. " +
                             "Locate the “return shipping packet”, included in one of the transporters (i.e. boxes or case), and follow the instructions for shipping. " +
                             "This packet contains shipping labels, outbound “Bill of Lading” documents, and contact information in case you have trouble. " +
                             "You will need to go to the show services desk or the show management to confirm final shipping instructions. Usually, " +
                             "“material handling” will pick up your labeled shipment directly in the booth. ");

            EmailBody.Append("<br/><br/><br/>");
            EmailBody.Append("Have a great show!");

            EmailBody.Append("<br/><br/><br/>");
            EmailBody.Append("<b>Kristin Hatteberg</b>");
            EmailBody.Append("<br/>");
            EmailBody.Append("Daikin Event Manager");

            return EmailBody;
        }

	}
}