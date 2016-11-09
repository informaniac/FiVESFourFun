﻿using System;
using RESTServicePlugin;
using System.IO;

namespace DiagnosisPlugin
{
    class DiagnosisRequestHandler : RequestHandler
    {
        public DiagnosisRequestHandler()
        {
        }

        public override string ContentType
        {
            get
            {
                return "text/html";
            }
        }

        public override string Path
        {
            get
            {
                return "/diagnosis";
            }
        }

        protected override RequestResponse HandleDELETE(string requestPath)
        {
            throw new NotImplementedException();
        }

        protected override RequestResponse HandleGET(string requestPath)
        {
            string sanitizedRequestPath = sanitizeRequestPath(requestPath);
            RequestResponse reqResponse = createResponse(sanitizedRequestPath);

            return reqResponse;
        }

        private string sanitizeRequestPath(string requestPath)
        {
            // register routes here. map return value to file which shall be returned
            string sanitizedPath = null;
            // TODO: redirects. some of the index paths wont work properly
            if(requestPath == "" || requestPath == "/" || requestPath == "/index" || requestPath == "/index/")
            {
                sanitizedPath = "DiagnosisWebpage/dynamic/index.html";
            }
            else if (requestPath.StartsWith("/DiagnosisWebpage/"))
            {
                sanitizedPath = requestPath.Substring(1);
                return sanitizedPath;
            }
            return sanitizedPath;
        }

        private RequestResponse createResponse(string requestPath)
        {
            // All accessible Files have to be in the DiagnosisWebpage folder
            RequestResponse reqResponse = new RequestResponse();

            reqResponse.ContentType = this.ContentType;
            string response;
            if (File.Exists(requestPath))
            {
                reqResponse.ContentType = getMimeType(requestPath);
                response = File.ReadAllText(requestPath);
                reqResponse.ReturnCode = 200;
            }
            else
            {
                response = "no such route";
                reqResponse.ReturnCode = 404;
            }

            if (requestPath.Contains("/dynamic/"))
                response = responseBuilder.RenderResponse().OuterXml;

            reqResponse.SetResponseBuffer(response);
            return reqResponse;
        }

        private string getMimeType(string requestPath)
        {
            if (requestPath.EndsWith(".html")){
                return "text/html";
            }
            else if (requestPath.EndsWith(".css"))
            {
                return "text/css";
            }
            else if (requestPath.EndsWith(".js"))
            {
                return "application/javascript";
            }
            else if (requestPath.EndsWith(".ttf"))
            {
                return "application/octet-stream";
            }
            else if (requestPath.EndsWith(".woff"))
            {
                return "application/x-font-woff";
            }
            else if (requestPath.EndsWith(".woff2"))
            {
                return "application/font-woff2";
            }
            else
            {
                return null;
            }
        }

        protected override RequestResponse HandlePOST(string requestPath, string content)
        {
            RequestResponse response = new RequestResponse();
            if (!requestPath.StartsWith("/action"))
            {
                response.ReturnCode = 400;
                string returnMessage = "So net";
            }

            throw new NotImplementedException();
        }

        protected override RequestResponse HandlePUT(string requestPath, string content)
        {
            throw new NotImplementedException();
        }

        private XmlResponseBuilder responseBuilder = new XmlResponseBuilder();
    }
}
