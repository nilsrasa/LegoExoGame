namespace LogModule
{
    public abstract class LogEntry
    {
        public string Header { get; protected set; }
        public abstract string ToCSV();

        public abstract string ToText();

    }
}
