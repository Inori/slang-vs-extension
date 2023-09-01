using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace SlangExtension
{
    [ContentType("slang")]
    [Export(typeof(ILanguageClient))]
    [RunOnContext(RunningContext.RunOnHost)]
    internal class SlangCustomLanguageClient : SlangLanguageClient, ILanguageClientCustomMessage2
    {
        public object MiddleLayer => LoggerMiddleLayer.Instance;

        public object CustomMessageTarget => null;

        private JsonRpc customMessageRpc;

        public SlangCustomLanguageClient() : base()
        {
            Instance = this;
        }

        public async Task AttachForCustomMessageAsync(JsonRpc rpc)
        {
            await Task.Yield();

            this.customMessageRpc = rpc;
        }

        internal class LoggerMiddleLayer : ILanguageClientMiddleLayer
        {
            internal readonly static LoggerMiddleLayer Instance = new LoggerMiddleLayer();
            private LoggerMiddleLayer() { }

            public bool CanHandle(string methodName)
            {
                return true;
            }

            public async Task<JToken> HandleRequestAsync(string methodName, JToken methodParam, Func<JToken, Task<JToken>> sendRequest)
            {
                Debug.WriteLine("Request: " + methodName + "->" + methodParam.ToString());

                return await sendRequest(methodParam);
            }

            public async Task HandleNotificationAsync(string methodName, JToken methodParam, Func<JToken, Task> sendNotification)
            {
                Debug.WriteLine("Notification: " + methodName + "->" + methodParam.ToString());

                await sendNotification(methodParam);
            }
        }
    }
}
