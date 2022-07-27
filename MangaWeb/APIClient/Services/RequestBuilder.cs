using RestSharp;

namespace MangaWeb.APIClient.Services
{
    public class RequestBuilder
    {
        private RestRequest? Request;

        public RequestBuilder CreateRequest()
        {
            if (Request == null)
            {
                Request = new RestRequest();
            }

            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method, string parameterName, string parameterValue)
        {
            SetRequestResource(resource);
            SetRequestMethod(method);
            AddRequestParameter(parameterName, parameterValue);
            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method, Parameter parameter)
        {
            SetRequestResource(resource);
            SetRequestMethod(method);
            AddRequestParameter(parameter);
            return this;
        }

        public RequestBuilder SetRequest(string resource, Method method)
        {
            SetRequestResource(resource);
            SetRequestMethod(method);
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

        public RequestBuilder SetRequestMethod(Method method)
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
