using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceClaimsApi.PolicyRegister;

    public class PolicyRegisterResponse
    {
        //public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
