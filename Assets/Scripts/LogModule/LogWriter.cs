using System.Collections.Generic;
using System.IO;

namespace LogModule
{
    /// <summary>
    /// Used to handle multiple log streams and log the entries
    /// </summary>
    public class LogWriter
    {
        private string _path;
        private Dictionary<string, LogStream> _streamLib;


        public LogWriter(string path)
        {
            _path = path;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            _streamLib = new Dictionary<string, LogStream>();
        }

        /// <summary>
        /// Logs the given logentry using the corresponding stream. If no exists yet, a new stream is created.
        /// </summary>
        /// <param name="entry">The logentry to be logged</param>
        public void LogEntry(LogEntry entry)
        {
            LogStream logStream;

            if (!_streamLib.ContainsKey(entry.Id))
            {
                logStream = new LogStream(_path, entry.Name, entry.Header);
                _streamLib.Add(entry.Id, logStream);
            }
            else
                logStream = _streamLib[entry.Id];

            logStream.WriteLine(entry.ToCSV());
        }

        public void Close()
        {
            //Close all the streams
            foreach (LogStream logStream in _streamLib.Values)
                logStream.Close();

            //Clear the library
            _streamLib.Clear();
        }

        private class LogStream
        {
            private StreamWriter _writer;
            private  int _lines;
            private string _path;

            public LogStream(string path, string filename, string header)
            {
                _path = path + filename + ".csv";
                _writer = new StreamWriter(_path);
                _writer.WriteLine(header);
            }

            public void WriteLine(string line)
            {
                _writer.WriteLine(line);
                _lines++;
            }

            public void Close()
            {
                _writer.Close();
            }
        }
    }
}
