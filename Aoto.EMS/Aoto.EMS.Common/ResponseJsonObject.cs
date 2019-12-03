using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Common
{
    public class Business
    {
        public string BusinessType { get; set; }
        public string BusinessNameNo { get; set; }
        public string BusinessName { get; set; }
        public string RealBusiness { get; set; }

    }
    public class ResponseJsonObject
    {
        public string[] InsuranceType { get; set; }
        public List<Business> businesslist { get; set; }

    }
}
