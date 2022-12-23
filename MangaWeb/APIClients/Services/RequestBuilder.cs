using RestSharp;

namespace MangaWeb.APIClients.Services
{
    public class RequestBuilder
    {
        private RestRequest? Request;
        public static RestRequest Get = new RestRequest();
        public RequestBuilder CreateRequest()
        {
            if (Request == null)
            {
                Request = new RestRequest();
            }

            return this;
        }

        public RequestBuilder AddHeadersForShikimori()
        {
            Request.AddHeader("Accept", "*/*")
                .AddHeader("Accept-Encoding", "gzip, deflate, br")
                .AddHeader("Cache-Control", "no-cache");

            return this;
        }

        public RequestBuilder AddJsonBody(object obj)
        {
            Request.RequestFormat = DataFormat.Json;
            Request.AddJsonBody(obj);
            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method, string parameterName, string parameterValue)
        {
            SetRequestResource(resource);
            AddRequestMethod(method);
            AddRequestParameter(parameterName, parameterValue);
            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method, Parameter parameter)
        {
            SetRequestResource(resource);
            AddRequestMethod(method);
            AddRequestParameter(parameter);
            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method)
        {
            SetRequestResource(resource);
            AddRequestMethod(method);
            return this;
        }

        public RequestBuilder SetRequestResource(string resource)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            Request.Resource = resource;
            return this;
        }
        public RequestBuilder AddRequestHeader(string header, string value)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }

            Request.AddHeader(header, value);
            return this;
        }

        public RequestBuilder AddRequestMethod(Method method)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            Request.Method = method;
            return this;
        }

        public RequestBuilder AddRequestParameter(Parameter parameter)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            Request.AddParameter(parameter);
            return this;
        }

        public RequestBuilder AddRequestHeaderParameter(string name, string value)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            Request.AddHeader(name, value);
            return this;
        }

        public RequestBuilder AddRequestParameter(string name, string value)
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            Request.AddParameter(name, value);
            return this;
        }

        public RestRequest GetRequest()
        {
            if (Request is null)
            {
                throw new ArgumentNullException(nameof(Request));
            }
            var _request = Request;
            ResetRequest();
            return _request;
        }

        public void ResetRequest()
        {
            Request = new RestRequest();
        }
    }
}
