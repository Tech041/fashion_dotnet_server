using EcommerceServer.Data;
using EcommerceServer.Dtos;
using EcommerceServer.Entities;
using EcommerceServer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceServer.Services
{
    public class VisitorTrackerService(AppDbContext context):IVisitorTracker
    {
       

        public async Task<bool> TrackVisitorAsync(string visitorId)
        {
            if (string.IsNullOrEmpty(visitorId))
                throw new ArgumentException("Missing visitorId");

            var today = DateTime.UtcNow.Date;

            var existing = await context.Visitors
                .FirstOrDefaultAsync(v => v.VisitorId == visitorId && v.Date == today);

            if (existing == null)
            {
                var visitor = new Visitor
                {
                    VisitorId = visitorId,
                    Date = today,
                    CreatedAt = DateTime.UtcNow
                };

                context.Visitors.Add(visitor);
                await context.SaveChangesAsync();
                return true;
            }
            return true;

        }

        public async Task<VisitorStat> GetVisitorStatsAsync(int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            var today = DateTime.UtcNow.Date;

            var dailyUnique = await context.Visitors
                .Where(v => v.Date == today)
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            var totalUnique = await context.Visitors
                .Select(v => v.VisitorId)
                .Distinct()
                .CountAsync();

            var groupedVisits = await context.Visitors
                .GroupBy(v => v.Date)
                .Select(g => new { Date = g.Key, TotalVisits = g.Count() })
                .OrderByDescending(g => g.Date)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var totalDays = await context.Visitors
                .Select(v => v.Date)
                .Distinct()
                .CountAsync();

            var totalPages = (int)Math.Ceiling(totalDays / (double)limit);

            return new VisitorStat
            {
                DailyUniqueVisitors = dailyUnique,
                TotalUniqueVisitors = totalUnique,
                Visits = groupedVisits.Select(v => new DailyVisit
                {
                    Date = v.Date,
                    TotalVisits = v.TotalVisits
                }).ToList(),
                TotalPages = totalPages
            };
        }
    }
}
