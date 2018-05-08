using DPO.Common;
using DPO.Domain;

namespace DPO.Services.Light
{
    public interface IOrderServiceLight
    {
        ServiceResponse GetNewOrder(UserSessionModel user, long quoteId);
        ServiceResponse GetSubmittedOrder(UserSessionModel user, long quoteId);
        ServiceResponse GetOrderInQuote(UserSessionModel user, long quoteId);
        ServiceResponse GetOrderStatusTypes(UserSessionModel user);
        ServiceResponse GetOrderOptions(UserSessionModel user, long? projectId, long? currentQuoteId);
    }
}
