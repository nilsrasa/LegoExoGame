using System.IO;

namespace LogModule
{
    public class LogWriter
    {
        private string _path;
        private StreamWriter _writer;
        private int lines;


        public LogWriter(string path, string header)
        {
            _path = path;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            _writer = new StreamWriter(path);
            _writer.WriteLine(header);
        }

        public void LogEntry(LogEntry entry)
        {
            _writer.WriteLine(entry.ToCSV());
            lines++;
        }

        public void EndTask()
        {
            _writer.Close();

            if (lines < 1)
                File.Delete(_path);
        }
    }
}
