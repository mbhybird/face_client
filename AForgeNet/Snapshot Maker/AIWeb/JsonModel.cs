using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIWeb
{
    public class Result<T>
    {
        public int status { get; set; }
        public T result { get; set; }
    }

    public class ErrorResult
    {
        public int error_code { get; set; }
        public string error_msg { get; set; }
    }

    public class OkResult
    {
       public string ok_msg { get; set; }
    }

    public class GetUserDataResult
    {
        public int id { get; set; }
        public string uid { get; set; }
        public string groupid { get; set; }
        public string userinfo { get; set; }
        public string username { get; set; }
        public Nullable<int> age { get; set; }
        public string gender { get; set; }
        public string race { get; set; }
        public Nullable<int> glasses { get; set; }
    }
    public class GetQueueResult
    {
        public int age { get; set; }
        public string gender { get; set; }
        public string groupid { get; set; }
        public string race { get; set; }
        public string uid { get; set; }
        public string type { get; set; }
        public string userinfo { get; set; }
        public int glasses { get; set; }
    }
}