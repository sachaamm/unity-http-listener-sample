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


namespace UnityCustomHttpListener.Demo.Controller
{
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

        /// <summary>
        /// Get one parameter in query string example 
        /// </summary>
        /// Request : GET http://localhost:4444/getOneParameterInQueryStringExample/?name=Bob
        /// <param name="request"></param>
        /// <returns></returns>
        [MyRestRoute("/getOneParameterInQueryStringExample/", HttpRestMethod.GET,HttpResponseUtility.HttpResponseContentType.Html)]
        public HttpResponseTemplate GetOneQueryParameterExampleRoute(HttpListenerRequest request)
        {
            string nameValue = "Somebody";
            
            var nameValuesFromQueryString = request.QueryString.GetValues("name");

            if (nameValuesFromQueryString != null && nameValuesFromQueryString.Length > 0)
            {
                nameValue = nameValuesFromQueryString[0]; // We expect to get only one value in "name" from the query string
            }
      
            string responseString = $"<HTML><BODY> Hello <H1>{nameValue}!</H1> </BODY></HTML>";
            return HttpResponseUtility.Ok(responseString);
        }
        
        /// <summary>
        /// Get one parameter in query string example 
        /// </summary>
        /// Request : GET http://localhost:4444/getPersonWithAgeAndNameInQueryParams/?age=22&name=Bob%20O%27Neill
        /// <param name="request"></param>
        /// <returns></returns>
        [MyRestRoute("/getPersonWithAgeAndNameInQueryParams/", HttpRestMethod.GET,HttpResponseUtility.HttpResponseContentType.Json)]
        public HttpResponseTemplate GetPersonFromIdWithQueryParams(HttpListenerRequest request)
        {
            
            List<Person> persons = new List<Person>();
            Person bob = new Person();
            bob.Age = 22;
            bob.Name = "Bob O'Neill";

            Person alice = new Person();
            alice.Age = 31;
            alice.Name = "Alice Reid";

            persons.Add(bob);
            persons.Add(alice);
            
            var ageValuesFromQueryString = request.QueryString.GetValues("age");
            var nameValuesFromQueryString = request.QueryString.GetValues("name");

            Person p = null;
            
            int ageValue = 0;
            string nameValue = "";

            if (ageValuesFromQueryString != null && ageValuesFromQueryString.Length > 0 && 
                nameValuesFromQueryString != null && nameValuesFromQueryString.Length > 0)
            {
                ageValue = int.Parse(ageValuesFromQueryString[0]); // We expect to get only one value in "name" from the query string
                nameValue = nameValuesFromQueryString[0];
                p = persons.FirstOrDefault(pp => pp.Age == ageValue && pp.Name == nameValue);
            }
            
            if (p != null)
            {
                return HttpResponseUtility.Ok(JsonUtility.ToJson(p));
            }
            else
            {
                p = new Person();
                p.Age = ageValue;
            }
            
            return HttpResponseUtility.NotFound(JsonUtility.ToJson(p));
        }
        
        /// <summary>
        /// Get one parameter in query string example 
        /// </summary>
        /// Request : POST http://localhost:4444/addPerson
        /// <param name="request"></param>
        /// <returns></returns>
        [MyRestRoute("/addPerson/", HttpRestMethod.POST,HttpResponseUtility.HttpResponseContentType.Json)]
        public HttpResponseTemplate AddPerson(HttpListenerRequest request)
        
        {
            string data_text = new StreamReader (request.InputStream, 
                request.ContentEncoding).ReadToEnd();
            
            string resultDataText =   data_text;

            Person p = JsonUtility.FromJson<Person>(resultDataText);
            
            
            return HttpResponseUtility.Created(data_text);
        }
        
        /// <summary>
        /// Get one parameter in query string example 
        /// </summary>
        /// Request : POST http://localhost:4444/addPersonAsync
        /// <param name="request"></param>
        /// <returns></returns>
        [MyRestRoute("/addPersonAsync/", HttpRestMethod.POST,HttpResponseUtility.HttpResponseContentType.Json)]
        public async Task<HttpResponseTemplate> AddPersonAsync(HttpListenerRequest request)
        {
            Task<string> data_text = new StreamReader (request.InputStream, 
                request.ContentEncoding).ReadToEndAsync();
            
            string resultDataText = await data_text;

            Person p = JsonUtility.FromJson<Person>(resultDataText);
            
            return HttpResponseUtility.Created(resultDataText);
        }

        
        
    }
}