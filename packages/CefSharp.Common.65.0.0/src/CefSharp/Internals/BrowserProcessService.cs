// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System.ServiceModel;

namespace CefSharp.Internals
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode=ConcurrencyMode.Multiple)]
    internal class BrowserProcessService : IBrowserProcess
    {
        private readonly JavascriptObjectRepository javascriptObjectRepository;
        private readonly BrowserProcessServiceHost host;
        
        public BrowserProcessService()
        {
            var context = OperationContext.Current;
            host = (BrowserProcessServiceHost)context.Host;

            javascriptObjectRepository = host.JavascriptObjectRepository;
        }

        public BrowserProcessResponse CallMethod(long objectId, string name, object[] parameters)
        {
            object result;
            string exception;
            
            var success = javascriptObjectRepository.TryCallMethod(objectId, name, parameters, out result, out exception);

            return new BrowserProcessResponse { Success = success, Result = result, Message = exception };
        }

        public BrowserProcessResponse GetProperty(long objectId, string name)
        {
            object result;
            string exception;
            var success = javascriptObjectRepository.TryGetProperty(objectId, name, out result, out exception);

            return new BrowserProcessResponse { Success = success, Result = result, Message = exception };
        }

        public BrowserProcessResponse SetProperty(long objectId, string name, object value)
        {
            string exception;
            var success = javascriptObjectRepository.TrySetProperty(objectId, name, value, out exception);

            return new BrowserProcessResponse { Success = success, Result = null, Message = exception };
        }
    }
}
