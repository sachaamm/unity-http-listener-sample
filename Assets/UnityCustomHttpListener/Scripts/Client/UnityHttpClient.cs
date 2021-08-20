using System;
using System.Net.Http;
using UnityEngine;

namespace UnityCustomHttpListener.Scripts.Client
{
    public class UnityHttpClient : MonoBehaviour
    {
        public static readonly HttpClient Client = new HttpClient();

        public static UnityHttpClient Singleton;

        private void Awake()
        {
            Singleton = this;
        }
        
        
    }
}
