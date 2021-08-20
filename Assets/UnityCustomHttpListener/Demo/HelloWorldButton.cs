using System;
using System.Net.Http;
using UnityCustomHttpListener.Scripts.Client;
using UnityEngine;
using UnityEngine.UI;

namespace UnityCustomHttpListener.Demo
{
    public class HelloWorldButton : MonoBehaviour
    {
        
        [SerializeField] private InputField _inputField;
        
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0
        public async void Click()
        {
            try	
            {
                HttpResponseMessage response = await UnityHttpClient.Client.GetAsync(_inputField.text);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Debug.Log(responseBody);
            }
            catch(HttpRequestException e)
            {
                Debug.Log("\nException Caught!");	
                Debug.Log($"Message :{e.Message} ");
            }
        }
    }
}
