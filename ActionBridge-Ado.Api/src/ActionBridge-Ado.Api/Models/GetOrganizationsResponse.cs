namespace ActionBridge_Ado.Api.Models
{
    public class GetOrganizationsResponse
    {
        public ADOOrgValue Organization { get; set; } = new() { AccountId = "", AccountURI = "", AccountName = "" };
        public List<GetProjectsResponse> Projects { get; set; } = [];
    }
}