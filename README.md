# unity-http-listener
A custom HTTP REST webserver middleware for Unity ( without .NET ). Build easily your REST API and communicate between your applications via HTTP Requests. ( Unity 2020.2.4f1 )



## Test
Open Assets/TestScene. Run the scene. Open the postman collection in postman. You can run requests samples via Postman. For text/html content type responses you can also use your web-browser.

## Configure 


## Code Example 
````cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityCustomHttpListener.Demo.Model;
using UnityCustomHttpListener.Scripts.Attribute;
using UnityCustomHttpListener.Scripts.Model;
using UnityCustomHttpListener.Scripts.Utility;
using UnityEngine;

[MyApiController]
    public class ExampleController
    {
        /// <summary>
        /// Hello world example
        /// </summary>
        /// Request : GET http://localhost:4444/
        /// <param name="request"></param>
        /// <returns></returns>
        [MyRestRoute("/", HttpRestMethod.GET,HttpResponseUtility.HttpResponseContentType.Html)]
        public HttpResponseTemplate HomeRoute(HttpListenerRequest request)
        {
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            return HttpResponseUtility.Ok(responseString);
        }
        
        }
```

## Import in any Unity version
