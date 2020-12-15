namespace WikiApp.ViewModels
{
    public class StatsModel
    {
        public string User { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Requests { get; set; }
        public int AcceptedRequests { get; set; }
        public int Moderators { get; set; }
    }
}