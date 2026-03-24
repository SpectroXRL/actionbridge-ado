namespace ActionBridge_Ado.Api.Models
{
    public class ADOOrgResponse
    {
        public int Count { get; set; }
        public List<ADOOrgValue> Value { get; set; } = [];
    }
}