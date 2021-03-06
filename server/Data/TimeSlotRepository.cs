using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

using Logic.TimeSlots;
using Logic.Context;
using Logic.DTO;

namespace Data
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly ICLupContext _context;
        public TimeSlotRepository(ICLupContext context)
        {
            _context = context;
        }
        public async Task<(int, HttpCode)> CreateTimeSlot(TimeSlot timeSlot)
        {
            await _context.TimeSlots.AddAsync(timeSlot);

            await _context.SaveChangesAsync();

            return (timeSlot.Id, HttpCode.Created);
        }

        public async Task<HttpCode> DeleteTimeSlot(int timeSlotId)
        {
            TimeSlot timeSlot = await FindTimeSlotById(timeSlotId);

            if (timeSlot == null)
            {
                return HttpCode.NotFound;
            }

            _context.TimeSlots.Remove(timeSlot);

            await _context.SaveChangesAsync();

            return (HttpCode.Deleted);
        }

        public async Task<TimeSlot> FindTimeSlotById(int timeSlotId)
        {
            TimeSlot timeSlot = await _context.TimeSlots.Include(x => x.Bookings)
                                                        .Include(x => x.Business)
                                                        .FirstOrDefaultAsync(x => x.Id == timeSlotId);

            return timeSlot;
        }

        public async Task<IList<TimeSlot>> FindTimeSlotsByBusiness(int businessId)
        {
            IList<TimeSlot> timeSlots = await _context.TimeSlots.Include(x => x.Bookings)
                                                                .Include(x => x.Business)
                                                                .Where(x => x.BusinessId == businessId)
                                                                .ToListAsync();
            return timeSlots;
        }

        public async Task<IList<TimeSlot>> FindTimeSlotsByBusinessAndDate(int businessId, DateTime date)
        {
            IList<TimeSlot> timeSlots = await _context.TimeSlots.Include(x => x.Bookings)
                                                                .Include(x => x.Business)
                                                                .Where(x => x.BusinessId == businessId && x.Start.Date == date.Date)
                                                                .ToListAsync();
            return timeSlots;                                                
        }

        public async Task<IList<TimeSlot>> FindAvailableTimeSlotsByBusiness(AvailableTimeSlotsRequest request)
        {
            IList<TimeSlot> timeSlots = await _context.TimeSlots.Include(x => x.Bookings)
                                                                .Include(x => x.Business)
                                                                .Where(x => x.BusinessId == request.BusinessId &&
                                                                               (x.Start > request.Start && x.End < request.End))
                                                                .ToListAsync();

            return timeSlots;
        }

        public async Task<HttpCode> UpdateTimeSlot(TimeSlot timeSlot)
        {
            TimeSlot ts = await FindTimeSlotById(timeSlot.Id);

            if (ts == null)
            {
                return HttpCode.NotFound;
            }

            ts.Bookings = timeSlot.Bookings;
            ts.BusinessId = timeSlot.BusinessId;
            ts.BusinessName = timeSlot.BusinessName;
            ts.Capacity = timeSlot.Capacity;
            ts.Start = timeSlot.Start;
            ts.End = timeSlot.End;

            await _context.SaveChangesAsync();

            return HttpCode.Updated;
        }
    }
}