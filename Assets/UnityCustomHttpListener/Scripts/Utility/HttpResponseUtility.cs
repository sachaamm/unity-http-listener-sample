using System.Collections.Generic;
using UnityCustomHttpListener.Scripts.Model;
using UnityEngine;

namespace UnityCustomHttpListener.Scripts.Utility
{

    public static class HttpResponseUtility
    {
        public enum HttpResponseContentType
        {
            PlainText,
            Json,
            Html
        }

        [System.Serializable]
        public class HttpResponseBody
        {
            public string Result;
            public int a;
        }

        public static Dictionary<HttpResponseContentType, string> ContentTypeToMimeTypeMap =
            new Dictionary<HttpResponseContentType, string>()
            {
                {HttpResponseContentType.PlainText, "text/plain"},
                {HttpResponseContentType.Json, "application/json"},
                {HttpResponseContentType.Html, "text/html"},
            };

        static byte[] MSG(string s)
        {
            HttpResponseBody msg = new HttpResponseBody();
            msg.Result = s;
            msg.a = 55;
            return System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(msg));
        }

        static byte[] BuildResponseContent(string content)
        {
            return System.Text.Encoding.UTF8.GetBytes(content);
        }

    public static HttpResponseTemplate Ok(string content)
        {
            HttpResponseTemplate template = new HttpResponseTemplate();
            template.statusCode = 200;
            template.content = BuildResponseContent(content);
            
            return template;
        }
    
    public static HttpResponseTemplate Created(string content)
    {
        HttpResponseTemplate template = new HttpResponseTemplate();
        template.statusCode = 201;
        template.content = BuildResponseContent(content);
            
        return template;
    }
        
        public static HttpResponseTemplate BadRequest(string content)
        {
            HttpResponseTemplate template = new HttpResponseTemplate();
            template.statusCode = 400;
            template.content = BuildResponseContent(content);
            
            return template;
        }
        
        public static HttpResponseTemplate Forbidden(string content)
        {
            HttpResponseTemplate template = new HttpResponseTemplate();
            template.statusCode = 403;
            template.content = BuildResponseContent(content);
            
            return template;
        }
        
        public static HttpResponseTemplate NotFound(string content)
        {
            HttpResponseTemplate template = new HttpResponseTemplate();
            template.statusCode = 404;
            template.content = BuildResponseContent(content);
            
            return template;
        }

        
    }
    
    
    
    
}