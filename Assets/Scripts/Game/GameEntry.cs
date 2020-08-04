using LogModule;
using System;

namespace Game
{
    public class GameEntry : LogEntry
    {
        public override string Header => "Timestamp, Score, Hits, Misses";

        public override string Id => Name;

        public override string Name => "GameLog";

        public string Timestamp { get; private set; }
        public int Score { get; private set; }
        public int Hits { get; private set; }
        public int Misses { get; private set; }


        public GameEntry(int score, int hits, int misses)
        {
            Timestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
            Score = score;
            Hits = hits;
            Misses = misses;
        }

        public override string ToCSV()
        {
            return $"\"{Timestamp}\",{Score},{Hits},{Misses}";
        }

        public override string ToText()
        {
            return $"Time: {Timestamp}, Score: {Score}, Hits: {Hits}, Misses: {Misses}";
        }
    }
}
