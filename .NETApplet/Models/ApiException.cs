using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;


namespace NETApplet
{
    public class ApiException : Exception
    {
        public long StatusCode { get; set; }
        public string Error { get; set; }

        public ApiException(System.Net.WebException e) : this(ResponseToDictionary(e.Response))
        {
        }

        public ApiException(Dictionary<string, object> dic) : base((string)dic["message"])
        {
            this.StatusCode = (long)dic["statusCode"];
            this.Error = (string)dic["error"];
        }

        private static Dictionary<string, object> ResponseToDictionary(WebResponse res)
        {
            using (var streamReader = new StreamReader(res.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());
            }
        }
    }
}
