using System.Collections.Generic;
using CLup.Domain.Businesses;
using CLup.Domain.Users;

namespace CLup.Application.Users.Queries.Responses
{
    public class FetchMessagesResponse
    {
        public string UserId { get; set; }
        
        public IList<UserMessage> SentMessages { get; set; }

        public IList<BusinessMessage> ReceivedMessages { get; set; }
    }
}