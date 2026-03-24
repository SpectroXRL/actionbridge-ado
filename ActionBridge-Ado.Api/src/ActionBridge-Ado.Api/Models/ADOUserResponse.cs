using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActionBridge_Ado.Api.Models
{
    public class ADOUserResponse
    {
        public required string DisplayName { get; set; }
        public required string EmailAddress { get; set; }
        public required string Id { get; set; }
    }
}