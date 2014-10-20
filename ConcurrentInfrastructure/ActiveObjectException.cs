using System;

namespace ConcurrentInfrastructure
{
    public class ActiveObjectException : Exception
    {
        public Exception Exception { get; set; }
        public string RequestStackTrace { get; set; }
        public DateTime RequestTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Original Stack Trace: {1}, Requested at {2}", Exception, RequestStackTrace,
                RequestTime.ToShortTimeString());
        }
    }
}
