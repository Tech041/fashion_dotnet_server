namespace EcommerceServer.Dtos
{
    public class VisitorStat
    {
        public int DailyUniqueVisitors { get; set; }
        public int TotalUniqueVisitors { get; set; }
        public List<DailyVisit> Visits { get; set; } = new();
        public int TotalPages { get; set; }
    }
}
