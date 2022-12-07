using System;
using System.Diagnostics;

internal sealed class GameClock
{
    private long Count;
    private const long NSFactor = 10000000L;

    public bool IsRunning { get; private set; }

    internal long Frequency { get { return Stopwatch.Frequency; } }

    internal bool isHighResolution { get { return Stopwatch.IsHighResolution; } }

    internal static long Timestamp { get { return Stopwatch.GetTimestamp(); } }

    internal TimeSpan Elapsed { get; private set; }

    internal TimeSpan Total { get; private set; }


    #region Benchmarking
    internal bool BenchmarkingEnabled { get; private set; }

    internal TimeSpan ElapsedMax { get; private set; }

    internal List<double> Frametimes { get; private set; }
    #endregion

    internal GameClock(bool enableBenchmarking)
    {
        if (enableBenchmarking)
        {
            BenchmarkingEnabled = true;
            Frametimes = new List<double>(36000);
        }

        Reset();
    }

    internal void Start()
    {
        IsRunning = true;
        Count = Timestamp;
    }

    internal void Stop()
    {
        IsRunning = false;
    }

    internal void Reset()
    {
        Count = Timestamp;
        Elapsed = TimeSpan.Zero;
        Total = TimeSpan.Zero;
        IsRunning = false;
    }

    internal void Restart()
    {
        Reset();
        Start();
    }

    internal void Step()
    {
        if (IsRunning)
        {
            long last = Count;
            Count = Timestamp;
            long offset = Count - last;
            Elapsed = DeltaToTimeSpan(offset);

            if (Elapsed > ElapsedMax)
                ElapsedMax = Elapsed;

            if (BenchmarkingEnabled)
                Frametimes.Add(Elapsed.TotalMilliseconds);

            Total += Elapsed;
        }
    }

    private TimeSpan DeltaToTimeSpan(long delta)
    {
        return TimeSpan.FromTicks((delta * NSFactor) / Frequency);
    }
}