using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CLup.Application.Extensions;
using CLup.Application.Shared;
using CLup.Application.Users.Queries.Responses;
using CLup.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CLup.Application.Users.Queries
{
    public class FetchMessagesHandler : IRequestHandler<FetchMessagesQuery, Result<FetchMessagesResponse>>
    {
        private readonly CLupContext _context;

        public FetchMessagesHandler(CLupContext context) => _context = context;
        
        public async Task<Result<FetchMessagesResponse>> Handle(
            FetchMessagesQuery query,
            CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == query.UserEmail)
                .ToResult()
                .EnsureDiscard(u => u.Id == query.UserId, "You don't have access to these messages")
                .AndThen(async () =>
                {
                    var sentMessages =
                        await _context.UserMessages.Where(um => um.SenderId == query.UserId).ToListAsync();
                    var receivedMessages = await _context.BusinessMessages.Where(bm => bm.ReceiverId == query.UserId)
                        .ToListAsync();

                    return new FetchMessagesResponse()
                    {
                        UserId = query.UserId,
                        SentMessages = sentMessages,
                        ReceivedMessages = receivedMessages
                    };
                })
                .Finally(response => response);
        }
    }
}