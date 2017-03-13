﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using SwaggerWcf.Models;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    public delegate Stream GetFileCustomDelegate(string filename, out string contentType, out long contentLength);

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SwaggerWcfEndpoint : SwaggerWcfEndpointBase
    {
        private static Service Service { get; set; }
        private static int _initialized;

        public SwaggerWcfEndpoint()
        {
            Init();
        }

        private static void Init()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            Service = ServiceBuilder.Build();
        }

        public static void Configure(Info info, SecurityDefinitions securityDefinitions = null)
        {
            Init();
            Service.Info = info;
            Service.SecurityDefinitions = securityDefinitions;
        }

        public override Stream GetSwaggerFile()
        {
            WebOperationContext woc = WebOperationContext.Current;
            if (woc != null)
            {
                //TODO: create a parameter in settings to configure this
                woc.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                woc.OutgoingResponse.ContentType = "application/json";
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Process(Service)));
        }
    }
}
