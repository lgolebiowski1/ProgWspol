using System.Collections.Concurrent;
using System.Text;

namespace TP.ConcurrentProgramming.Data
{
    /// <summary>
    /// Diagnostic entry — data for one ball position sample.
    /// </summary>
    public record DiagnosticEntry(
        DateTime Timestamp,
        double Radius,
        double X,
        double Y,
        double Vx,
        double Vy,
        double ElapsedMs);

    /// <summary>
    /// Abstraction for diagnostic logging — allows DI in tests.
    /// </summary>
    public interface IDiagnosticLogger : IDisposable
    {
        void Log(DiagnosticEntry entry);
    }

    /// <summary>
    /// Asynchronous file logger using a producer/consumer queue.
    /// Handles temporary lack of write throughput by buffering entries.
    /// Serializes data as ASCII text (CSV).
    /// </summary>
    internal class DiagnosticLogger : IDiagnosticLogger
    {
        private readonly BlockingCollection<DiagnosticEntry> _queue =
            new BlockingCollection<DiagnosticEntry>(boundedCapacity: 4096);

        private readonly Task _writerTask;
        private readonly string _filePath;
        private bool _disposed = false;

        internal DiagnosticLogger(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BilardDiagnostics.csv");

            // Start background writer task
            _writerTask = Task.Run(WriteLoop);
        }

        public void Log(DiagnosticEntry entry)
        {
            if (_disposed) return;
            // TryAdd — if queue is full, drop entry (handles lack of write throughput)
            _queue.TryAdd(entry);
        }

        private async Task WriteLoop()
        {
            try
            {
                await using StreamWriter writer = new StreamWriter(
                    _filePath, append: false,
                    encoding: Encoding.ASCII);

                // Write CSV header
                await writer.WriteLineAsync(
                    "Timestamp,Radius,X,Y,Vx,Vy,ElapsedMs");

                foreach (DiagnosticEntry entry in _queue.GetConsumingEnumerable())
                {
                    string line = string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0:O},{1:F2},{2:F4},{3:F4},{4:F4},{5:F4},{6:F2}",
                        entry.Timestamp,
                        entry.Radius,
                        entry.X,
                        entry.Y,
                        entry.Vx,
                        entry.Vy,
                        entry.ElapsedMs);

                    await writer.WriteLineAsync(line);

                    // Flush periodically — not every line (performance)
                    if (_queue.Count == 0)
                        await writer.FlushAsync();
                }
            }
            catch (Exception)
            {
                // Logging must never crash the application
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _queue.CompleteAdding();         // signal writer to finish
            _writerTask.Wait(2000);          // wait up to 2s for writer to flush
            _queue.Dispose();
        }
    }

    /// <summary>
    /// No-op logger for tests — discards all entries.
    /// </summary>
    public class NullDiagnosticLogger : IDiagnosticLogger
    {
        public void Log(DiagnosticEntry entry) { }
        public void Dispose() { }
    }
}
