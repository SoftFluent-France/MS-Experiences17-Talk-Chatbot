using BotDemo.Flows;
using RestSharp;

namespace BotDemo.Services
{
    public static class TicketManager
    {
        private const int ProjectId = 1;

        private const string PrivateToken = "f5Vv5Ez1pyx-4gaw5iwX";

        public static void CreateTicket(BugReport report)
        {
            var client = new RestClient("http://192.168.136.100");

            RestRequest request = new RestRequest($@"api/v4/projects/{ProjectId}/issues", Method.POST);
            request.AddHeader("PRIVATE-TOKEN", PrivateToken);

            request.AddParameter("title", report.Title);
            request.AddParameter("description", report.Description);
            request.AddParameter("labels", $"{CriticityToLabel(report.TicketCriticity)},{report.TicketType}");

            var rep = client.Execute(request);
        }

        private static string CriticityToLabel(TicketCriticity criticity)
        {
            switch (criticity)
            {
                case TicketCriticity.Lowest:
                    return "priorité 5";
                case TicketCriticity.Low:
                    return "priorité 4";
                default:
                case TicketCriticity.Medium:
                    return "priorité 3";
                case TicketCriticity.High:
                    return "priorité 2";
                case TicketCriticity.Highest:
                    return "priorité 1";
            }
        }
    }
}