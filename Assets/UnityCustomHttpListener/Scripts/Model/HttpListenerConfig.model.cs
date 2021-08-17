using System;
using System.Collections.Generic;

namespace UnityCustomHttpListener.Scripts.Model
{
    [Serializable]
    public class HttpListenerConfig
    {
        public List<string> urlBases;

        public HttpListenerConfig()
        {
            
        }
    }
}