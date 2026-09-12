using EcommerceServer.Dtos;

namespace EcommerceServer.Interfaces
{
    public interface IVisitorTracker
    {
        public  Task<bool> TrackVisitorAsync(string visitorId);
        public Task<VisitorStat> GetVisitorStatsAsync(int page, int limit);
    }
}
