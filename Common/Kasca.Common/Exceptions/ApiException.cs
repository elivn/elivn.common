using System;

namespace Kasca.Common.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string msg, int code = -1)
          : base(msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

        public int Code { get; set; }

        public string Msg{ get; set; }
    }
}
