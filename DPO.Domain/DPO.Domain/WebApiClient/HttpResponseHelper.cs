using DPO.Common;
using System.Net.Http;

namespace DPO.Domain 
{
    public class HttpResponseHelper : BaseServices
    {
        private readonly HttpResponseMessage httpResponseMsg;
        public const string http_failure = "Http Response Failure";
        public const string PONumberKey = "PONumber";
        public const string duplicate_ponumber_exists = "DuplicatePONumberExists";
        public const string ponumber_doesnot_exist = "PONumberDoesnotExist";
        public const string post_order_success = "PostOrderSuccessful";
        public const string post_order_failure = "PostOrderFailure";
 
        public HttpResponseHelper(HttpResponseMessage _httpResponseMsg)
        {
            httpResponseMsg = _httpResponseMsg;
        }

        public ServiceResponse GetOrderInfoApiResponse()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<ERPOrderInfo>();
            if (responseObject != null && responseObject.Result?.POKey != 0)
                GetAppropriateResponseMessage(duplicate_ponumber_exists); // not ok to proceed for persisting in DC db
            else
                GetAppropriateResponseMessage(ponumber_doesnot_exist); //Good to proceed, no duplicate PO Number exists in mapics
           
            return Response;
        }

        public OrderResponse[] GetOrdersToUpdateApiResponse()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<OrderResponse[]>();
            return (responseObject != null && responseObject.Result?.Length > 0) ? responseObject.Result : null;
        }

        public ERPOrderInfo[] GetSubmittedOrdersStatus()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<ERPOrderInfo[]>();
            return (responseObject != null && responseObject.Result?.Length > 0) ? responseObject.Result : null;
        }

        public OrderResponse[] GetAwaitingCSROrderStatus()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<OrderResponse[]>();
            return (responseObject != null && responseObject.Result?.Length > 0) ? responseObject.Result : null;
        }

        public ERPOrderInfo GetInProcessOrderStatus()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<ERPOrderInfo>();
            return (responseObject != null) ? responseObject.Result : null;
        }

        public ERPInvoiceInfo GetPickedOrderStatus()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<ERPInvoiceInfo>();
            return (responseObject != null) ? responseObject.Result : null;
        }

        public ERPInvoiceInfo[] GetInvoicesToUpdateApiResponse()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<ERPInvoiceInfo[]>();
            return (responseObject != null && responseObject.Result?.Length > 0) ? responseObject.Result : null;
        }

        public ServiceResponse GetPostOrdersApiResponse()
        {
            var responseObject = httpResponseMsg.Content.ReadAsAsync<OrderResult>();
            if (responseObject.Result != null && responseObject.Result.isSaved.Equals(true))
                GetAppropriateResponseMessage(post_order_success); // //successfully added in mapics
            else
                Response?.Messages?.AddError(PONumberKey, responseObject?.Result?.Message);
        
            return Response;
        }

        public ServiceResponse GetAppropriateResponseMessage(string message)
        {
            Response.Messages?.Clear();

            if (message == "GetOrderInfoCallFailed")
                Response.Messages?.AddError(PONumberKey, StrHttpMessage.PONumberCheckFailure);

            if (message == duplicate_ponumber_exists)
                Response.Messages?.AddError(PONumberKey, StrHttpMessage.DuplicatePONumberExists);

            if (message == ponumber_doesnot_exist)
                Response.Messages?.Add(MessageTypeEnum.Success, PONumberKey, StrHttpMessage.PONumberDoesnotExist);

            if (message == "PostOrderFailed")
                Response.Messages?.AddError(PONumberKey, StrHttpMessage.PostOrderFailure);

            if (message == post_order_success)
                Response.Messages?.Add(MessageTypeEnum.Success, PONumberKey, StrHttpMessage.PostOrderSuccess);

            return Response;
        }
    }

    public static class StrHttpMessage
    {
        public static string PONumberCheckFailure { get; } =  "Duplicate PO number check was unsuccessful.";
        public static string DuplicatePONumberExists { get; } = "PO Number already exists in Mapics.";
        public static string PONumberDoesnotExist { get; } = "PO number doesn't exist in Mapics.";
        public static string PostOrderFailure { get; } = "Posting orders to Mapics was unsuccessful.";
        public static string PostOrderSuccess { get; } = "Order with order details successfully saved to Mapics.";
    }
}
