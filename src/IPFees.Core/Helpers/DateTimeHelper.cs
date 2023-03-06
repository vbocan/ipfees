using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFees.Core.Helpers
{
    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime GetDateTimeNow() =>  DateTime.UtcNow.ToLocalTime();
    }
}
