using System;
using Logibit.Hawk;
using Newtonsoft.Json;
using Microsoft.FSharp.Core;
using NodaTime;
using System.Net;
using System.IO;

namespace NETApplet
{
    public class Api
    {
        private const string ProdUrl = "https://api.getmyle.com";
        private const string SandboxHostUrl = "https://dev.getmyle.com:444";

        private static Types.Credentials Credentials = new Types.Credentials(<applet rdns>, <applet secret>, Types.Algo.SHA256);

        public string Host { get; private set; }

        public Api(bool sandbox)
        {
            this.Host = sandbox ? SandboxHostUrl : ProdUrl;
        }

        public Ticket getTicket(string session)
        {
            var json = MakeRequest(this.Host + "/v1/ticket", Types.HttpMethod.POST, new { session }, Credentials);
            return JsonConvert.DeserializeObject<Ticket>(json);
        }

        public Record[] Query(Ticket ticket, object query)
        {
            var json = MakeRequest(this.Host + "/v1/query", Types.HttpMethod.POST, query, ticket);
            return JsonConvert.DeserializeObject<Record[]>(json);
        }


        private string MakeRequest(string url, Types.HttpMethod method, object bodyJson, Types.Credentials credentials)
        {
            var options = new Client.ClientOptions(
                credentials,
                SystemClock.Instance.Now,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            return MakeRequest(url, method, bodyJson, options);
        }


        private string MakeRequest(string url, Types.HttpMethod method, object bodyJson, Ticket ticket)
        {
            var options = new Client.ClientOptions(
                new Types.Credentials(ticket.Id, ticket.Key, Types.Algo.SHA256),
                SystemClock.Instance.Now,
                null,
                null,
                null,
                null,
                null,
                null,
                new FSharpOption<string>(ticket.App),
                null);
            return MakeRequest(url, method, bodyJson, options);
        }


        private string MakeRequest(string url, Types.HttpMethod method, object bodyJson, Client.ClientOptions options)
        {
            var authorizationHeader = Client.headerStr(url, method, options);
            var authorizationHeaderData = authorizationHeader as FSharpChoice<Client.HeaderData, Client.HeaderError>.Choice1Of2;

            var request = WebRequest.Create(url);
            request.Method = method.ToString();
            request.Headers.Add("Authorization", authorizationHeaderData.Item.header);
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                using (var tw = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(tw, bodyJson);
                }
            }
            try
            {
                var httpResponse = request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (System.Net.WebException e)
            {
                throw new ApiException(e);
            }
        }
    }
}
