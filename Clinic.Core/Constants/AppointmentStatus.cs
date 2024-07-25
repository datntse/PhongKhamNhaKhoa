using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Constants
{
    public class AppointmentStatus
    {
        public const int Pending = 0;
        public const int Approve = 1;
        public const int Reject = 2;
        public const int Booked = 3;
        public const int Cancel = 4;
        public const int Finish = 5;
        public const int All = 6;
    }
}
