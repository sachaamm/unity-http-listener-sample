using System.Collections.Generic;

namespace UnityCustomHttpListener.Scripts.Model
{
    public class MyPrefixesConfig
    {
        private readonly List<string> _urlBases;
        private readonly List<string> _routesLocalPaths;
        
        public MyPrefixesConfig(List<string> urlBases, List<string> routesLocalPaths)
        {
            this._urlBases = urlBases;
            this._routesLocalPaths = routesLocalPaths;
        }

        public List<string> GetRoutes()
        {
            List<string> routes = new List<string>();

            foreach (var urlBase in _urlBases)
            {
                foreach (var routeLocalPath in _routesLocalPaths)
                {
                    routes.Add(urlBase + routeLocalPath);
                }
            }

            return routes;
        }
    }
}