using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace InsuranceClaimsApi.Models.Helpers;

public class BaseResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
    
