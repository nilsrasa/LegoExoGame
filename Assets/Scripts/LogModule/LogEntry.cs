namespace LogModule
{
    public abstract class LogEntry
    {
        public abstract string Header { get; }
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string ToCSV();

        public abstract string ToText();

    }
}
