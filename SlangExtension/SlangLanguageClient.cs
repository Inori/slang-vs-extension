using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using StreamJsonRpc;
using System;
using System.IO;
using System.Linq;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell;


namespace SlangExtension
{
    public class SlangLanguageClient : ILanguageClient
    {
        public string Name => "Slang Language Extension";
        public object InitializationOptions => null;
        public bool ShowNotificationOnInitializeFailed => true;

        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;

        private const string SLANGD_EXECUTABLE_NAME = "slangd.exe";

        public IEnumerable<string> ConfigurationSections
        {
            get
            {
                yield return "slang";
            }
        }

        public IEnumerable<string> FilesToWatch => null;
        
        internal static SlangLanguageClient Instance
        {
            get;
            set;
        }
     

        public SlangLanguageClient()
        {
            
        }

        private string GetSlangDPath()
        {
            string slangBinFolder = Environment.GetEnvironmentVariable("PATH")
                .Split(';')
                .Where(d => File.Exists(Path.Combine(d, SLANGD_EXECUTABLE_NAME)))
                .FirstOrDefault();
            if (slangBinFolder == null) 
            {
                return null;
            }
            return Path.Combine(slangBinFolder, SLANGD_EXECUTABLE_NAME);
        }

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();

            var slangdPath = GetSlangDPath();
            if (slangdPath == null)
            {
                return null;
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = slangdPath;
            info.Arguments = "";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = info;

            if (process.Start())
            {
                return new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            if (StartAsync != null)
            {
                await StartAsync.InvokeAsync(this, EventArgs.Empty);
            }
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task<InitializationFailureContext> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            string message = "Slang Language Client failed to activate. :(";
            string exception = initializationState.InitializationException?.ToString() ?? string.Empty;
            message = $"{message}\n {exception}";

            var failureContext = new InitializationFailureContext()
            {
                FailureMessage = message,
            };

            return Task.FromResult(failureContext);
        }

    }
}
