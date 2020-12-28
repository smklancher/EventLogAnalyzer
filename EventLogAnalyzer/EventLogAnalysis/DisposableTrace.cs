using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UtilityCommon
{
    public class DisposableTrace : IDisposable
    {
        private const int SpacesPerTab = 3;

        [ThreadStatic]
        private static int tabLevel = 0;

        // To detect redundant calls
        private bool disposedValue;

        private string methodName = string.Empty;

        private Stopwatch stopwatch = new();

        /// <summary>
        /// Because of how overloads and optional params work, this can only take advantage of caller name by specifing Label as a named parameter:
        /// DisposableTrace(Label:"my label")
        /// </summary>
        /// <param name="Label"></param>
        /// <param name="CallerName"></param>
        public DisposableTrace(string Label, [CallerMemberName] string CallerName = null)
        {
            StartTrace($"{CallerName} - {Label}");
        }

        /// <summary>
        /// If wanting to add a separate label but still get callername, use this: DisposableTrace(Label:"my label")
        /// </summary>
        /// <param name="CallerNameOrLabel"></param>
        public DisposableTrace([CallerMemberName] string CallerNameOrLabel = null)
        {
            StartTrace(CallerNameOrLabel);
        }

        ~DisposableTrace()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(false);

            //base.Finalize();
        }

        public static void WriteEntry(string st)
        {
            string spacing = string.Empty.PadRight(SpacesPerTab * tabLevel);
            Trace.WriteLine(spacing + st);
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing).
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void StartTrace(string label)
        {
            methodName = label;
            WriteEntry($">> {methodName}");
            tabLevel += 1;
            stopwatch.Start();
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    stopwatch.Stop();
                    tabLevel -= 1;
                    WriteEntry($"<< {methodName} ({stopwatch.ElapsedMilliseconds} ms)");
                }
            }

            this.disposedValue = true;
        }
    }
}