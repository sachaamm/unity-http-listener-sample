using UnityCustomHttpListener.Scripts.Model;
using UnityCustomHttpListener.Scripts.Utility;

namespace UnityCustomHttpListener.Scripts.Attribute
{
    public class MyRestRouteAttribute : System.Attribute
    {
        private string name;

        public string Name => name;

        public HttpRestMethod Method => method;

        private HttpRestMethod method;

        private HttpResponseUtility.HttpResponseContentType _contentType;

        public HttpResponseUtility.HttpResponseContentType ContentType => _contentType;

        public MyRestRouteAttribute(string name, HttpRestMethod httpRestMethod, HttpResponseUtility.HttpResponseContentType contentType)  
        {  
            this.name = name;
            this.method = httpRestMethod;
            _contentType = contentType;
        }
    }
}