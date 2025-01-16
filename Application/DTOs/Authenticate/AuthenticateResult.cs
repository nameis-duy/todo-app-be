using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authenticate
{
#pragma warning disable CS8618
    public class AuthenticateResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
