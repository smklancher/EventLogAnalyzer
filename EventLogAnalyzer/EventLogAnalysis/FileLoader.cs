namespace EventLogAnalysis
{
    public class FileLoader : IDisposable
    {
        public const string SubFolderName = "EventLogAnalyzer";
        private bool disposedValue;

        public FileLoader(string filepath)
        {
            OriginalFile = new(filepath);
        }

        public FileInfo OriginalFile { get; }
        public FileInfo? TempFile { get; private set; }

        public void CopyToTemp()
        {
            var dest = Path.Combine(Path.GetTempPath(), SubFolderName, OriginalFile.Name);
            TempFile = new(dest);

            File.Copy(OriginalFile.FullName, dest, true);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)

                    try
                    {
                        TempFile?.Delete();
                    }
                    catch
                    {
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FileLoader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}