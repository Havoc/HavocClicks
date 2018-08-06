using System;
using System.Diagnostics;

namespace HavocClicks
{
    public class StatisticsRecorder
    {
        public delegate void UpdateStatisticsHandler(double clicksPerSecond, long totalClicks, long elapsedMilliseconds);
        public event UpdateStatisticsHandler UpdateStatistics;

        public long TotalClicks { get; private set; }
        public long ElapsedMilliseconds { get; private set; }
        public double ClicksPerSecond { get; private set; }

        public Stopwatch _stopWatch = new Stopwatch();

        public StatisticsRecorder()
        {
            TotalClicks = 0;
        }
        
        public void Start()
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        public void Stop() 
        {
            _stopWatch.Stop();
        }

        public void Finish(long clickCounter)
        {
            TotalClicks = clickCounter;
            ElapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
            double elapsedMillisecondsD = Convert.ToDouble(ElapsedMilliseconds);
            double totalClicksD = Convert.ToDouble(TotalClicks);
            ClicksPerSecond = (totalClicksD / elapsedMillisecondsD) * 1000;

            UpdateStatistics(ClicksPerSecond, TotalClicks, ElapsedMilliseconds);
            _stopWatch.Reset();
        }

        public void FinishTest(long clickCounter, long elapsedMilliseconds)
        {
            TotalClicks = clickCounter;
            ElapsedMilliseconds = elapsedMilliseconds;
            double elapsedMillisecondsD = Convert.ToDouble(ElapsedMilliseconds);
            double totalClicksD = Convert.ToDouble(TotalClicks);
            ClicksPerSecond = (totalClicksD / elapsedMillisecondsD) * 1000;

            UpdateStatistics(ClicksPerSecond, TotalClicks, ElapsedMilliseconds);
            //_stopWatch.Reset();
        }
    }
}
