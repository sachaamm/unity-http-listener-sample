using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityCustomHttpListener.Demo.Model;
using UnityCustomHttpListener.Scripts.Attribute;
using UnityCustomHttpListener.Scripts.Model;
using UnityCustomHttpListener.Scripts.Utility;
using UnityEngine;


// https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10

namespace UnityCustomHttpListener.Scripts.Router
{
    public class UnityHttpListener : MonoBehaviour
    {
        private HttpListener listener;
        private Thread listenerThread;
        
        public static UnityHttpListener Singleton;

        private List<HttpController> routerControllers = new List<HttpController>();

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            string path = Directory.GetCurrentDirectory() + "/http-listener-config.json";

            string json = File.ReadAllText(path);
            HttpListenerConfig config = JsonUtility.FromJson<HttpListenerConfig>(json);

            Init(config.urlBases);
        }
        
        public void Init(List<string> urlBases)
        {
            listener = new HttpListener ();

            Debug.Log($"Create routing local paths with {urlBases.Count} url bases  : ");

            foreach (var urlBase in urlBases)
            {
                Debug.Log(">>> <b>" + urlBase + "</b>");
            }
            
            var prefixConfig = new MyPrefixesConfig(urlBases, DeterminesRoutesToAttachFromReflection());
            
            InitListener(prefixConfig);
        }

        /// <summary>
        /// Starts the Http listener 
        /// </summary>
        /// <param name="prefixesConfig">The prefixes config is used to the router, it contains all router base url and all routes local paths.</param>
        void InitListener (MyPrefixesConfig prefixesConfig)
        {
            Debug.Log ("*** Server Started ***");
            
            foreach (var url in prefixesConfig.GetRoutes())
            {
                listener.Prefixes.Add (url);
            }

            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start ();

            listenerThread = new Thread (startListener);
            listenerThread.Start ();
            
        }
        
        /// <summary>
        /// Loop listening for incoming HTTP Requests.
        /// </summary>
        private void startListener ()
        {
            while (true) {               
                var result = listener.BeginGetContext (ListenerCallback, listener);
                result.AsyncWaitHandle.WaitOne ();
            }
        }

        /// <summary>
        /// Use assembly reflection to automatically add controllers routes to router ( don't forget to add MyApiController attributes on controllers )
        /// </summary>
        /// <returns>All local routes Paths</returns>
        List<string> DeterminesRoutesToAttachFromReflection()
        {
            List<string> routesLocalPaths = new List<string>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.GetCustomAttributes(typeof(MyApiControllerAttribute), true).Length > 0)
                    {
                        var methods = 
                            t.GetMethods()
                                .Where(m=>m.GetCustomAttributes(typeof(MyRestRouteAttribute), false).Length > 0)
                                .ToArray();
        
                        foreach (MethodInfo methodInfo in methods)
                        {
                            object[] attrs = methodInfo.GetCustomAttributes(typeof(MyRestRouteAttribute), false);
     
                            foreach (var attr in attrs)
                            {
                                if (attr is MyRestRouteAttribute)
                                {
                                    MyRestRouteAttribute mr = attr as MyRestRouteAttribute;

                                    HttpController httpController = new HttpController();
                                    httpController.MethodInfo = methodInfo;
                                    httpController.instance = Activator.CreateInstance(t);
                                    routerControllers.Add(httpController);
                                    
                                    routesLocalPaths.Add(mr.Name);
                                    Debug.Log($"Adding Route Local Path : <b>{mr.Name} <color=green>({mr.Method.ToString()})</color></b>");
                                }
                            }
            
                        }
                    }
                    
                    
                }
            }
            // var routerTypes = 
            
            

            return routesLocalPaths;
        }

        private async void ListenerCallback (IAsyncResult result)
        {				
            HttpListenerContext context = listener.EndGetContext (result);		

            Debug.Log ("Method: " + context.Request.HttpMethod);
            Debug.Log ("LocalUrl: " + context.Request.Url.LocalPath);

            if (context.Request.QueryString.AllKeys.Length > 0)
                foreach (var key in context.Request.QueryString.AllKeys) {
                    Debug.Log ("Query Parameter: " + key + ", Value: " + context.Request.QueryString.GetValues (key) [0]);
                }
            
            // Task<string> data_text = new StreamReader (context.Request.InputStream, 
            //     context.Request.ContentEncoding).ReadToEndAsync();
            //
            // string resultDataText = await data_text;
            // Debug.Log (resultDataText);
            
            HttpListenerResponse response = context.Response;

            try
            {
                Task<HttpResponse> httpResponseTask = GetResponseForUrl(context.Request, context.Request.Url.LocalPath,
                    context.Request.HttpMethod);

                HttpResponse httpResponse = await httpResponseTask;
                
                byte[] buffer = httpResponse.ResponseResult.content;
                response.ContentLength64 = buffer.Length;


                HttpResponseUtility.HttpResponseContentType contentType =
                    httpResponse.RouteAttributes?.ContentType ?? HttpResponseUtility.HttpResponseContentType.Html;
                response.ContentType = HttpResponseUtility.ContentTypeToMimeTypeMap[contentType];
                response.StatusCode = httpResponse.ResponseResult.statusCode;
                // response.

                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                context.Response.Close();
            }
            catch (Exception e)
            {
                response.ContentType = HttpResponseUtility.ContentTypeToMimeTypeMap[HttpResponseUtility.HttpResponseContentType.Html];
                response.StatusCode = 500;
                response.StatusDescription = "ERROR";

                string msg = "UNDEFINED ERROR, innerException is null";
                string stack = " UNDEFINED STACK ";
                if (e.InnerException != null)
                {
                    msg = e.InnerException.Message;
                    stack = e.InnerException.StackTrace;
                }
                
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes($"<HTML><BODY> <h3>{msg}</h3> <p> {stack}</p> </BODY></HTML>");
                
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                context.Response.Close();

            }
            
            // You must close the output stream.
            
        }

        async Task<HttpResponse> GetResponseForUrl(HttpListenerRequest request, string localUrl, string httpRestMethod)
        {
            var controllers = 
                routerControllers
                    .Where(m=>m.MethodInfo.GetCustomAttributes(typeof(MyRestRouteAttribute), false).Length > 0)
                    .ToArray();
            
            List<Task> tasks = new List<Task>();

                foreach (HttpController httpController in controllers)
                {
                    object[] attrs = httpController.MethodInfo.GetCustomAttributes(typeof(MyRestRouteAttribute), false);
                    
                    foreach (var attr in attrs)
                    {
                        if (attr is MyRestRouteAttribute)
                        {
                            MyRestRouteAttribute mr = attr as MyRestRouteAttribute;
                            if (mr.Name == localUrl && mr.Method.ToString() == httpRestMethod)
                            {
                                object[] parameters = new object[] {request};
                            
                                HttpResponse httpResponse = new HttpResponse();
                                httpResponse.RouteAttributes = mr;

                                object methodResult = httpController.MethodInfo.Invoke(httpController.instance, parameters);

                                if (methodResult.GetType() == typeof(HttpResponseTemplate))
                                {
                                    httpResponse.ResponseResult = methodResult as HttpResponseTemplate;
                                }
                                else
                                {
                                    httpResponse.ResponseResult = await (methodResult as Task<HttpResponseTemplate>);
                                }
                            
                                return httpResponse;

                            }
                        }
                    }
            
                }
          
                // await Task.CompletedTask;
            
            // await t.WaitUntil(isExcelInteractive);

            return NotFoundUrl(request);
        }

        HttpResponse NotFoundUrl(HttpListenerRequest request)
        {
            HttpResponse httpResponse = new HttpResponse();
            httpResponse.RouteAttributes = null;

            HttpResponseTemplate template = new HttpResponseTemplate();
            template.content = System.Text.Encoding.UTF8.GetBytes("<HTML><BODY> Not Found</BODY></HTML>");
            template.statusCode = 404;
            
            httpResponse.ResponseResult = template;
                

            return httpResponse;
        }
        
    }
}