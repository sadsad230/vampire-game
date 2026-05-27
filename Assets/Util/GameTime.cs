using UnityEngine;

#region ENGINE TIME MANAGER (With constructor-safe caching)
public static class GameTime
{
    private static float totalPauseTime;
    private static float pauseStartTime;
    private static bool isPaused;

    // Safe cache variables to bypass Unity's constructor restrictions
    private static float cachedTime;
    private static float cachedUnscaledTime;

    public static bool IsPaused
    {
        get => isPaused;
        set
        {
            if (isPaused == value) return;
            isPaused = value;

            float now = GetSafeTime();
            if (isPaused)
                pauseStartTime = now;
            else
                totalPauseTime += now - pauseStartTime;
        }
    }

    // Custom timeline used by game功 timers
    public static float CurrentTime
    {
        get
        {
            float now = GetSafeTime();
            return isPaused ? (pauseStartTime - totalPauseTime) : (now - totalPauseTime);
        }
    }

    public static float UnscaledTime => GetSafeUnscaledTime();

    // Safely retrieves Time.time even if called from a MonoBehaviour constructor
    private static float GetSafeTime()
    {
        try
        {
            float t = Time.time;
            cachedTime = t; // Refresh cache when allowed
            return t;
        }
        catch (System.Exception)
        {
            return cachedTime; // Fallback to the last known frame time during construction
        }
    }

    // Safely retrieves Time.unscaledTime even if called from a MonoBehaviour constructor
    private static float GetSafeUnscaledTime()
    {
        try
        {
            float t = Time.unscaledTime;
            cachedUnscaledTime = t;
            return t;
        }
        catch (System.Exception)
        {
            return cachedUnscaledTime;
        }
    }

    // Automatically spins up a invisible ticker to keep the cache fresh
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Reset();
        
        GameObject go = new GameObject("GameTime_SilentUpdater");
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave; // Hides it completely from the Hierarchy
        go.AddComponent<GameTimeUpdater>();
    }

    private class GameTimeUpdater : MonoBehaviour
    {
        void Update()
        {
            cachedTime = Time.time;
            cachedUnscaledTime = Time.unscaledTime;
        }
    }

    public static void Reset()
    {
        totalPauseTime = 0f;
        pauseStartTime = 0f;
        isPaused = false;
        cachedTime = 0f;
        cachedUnscaledTime = 0f;
    }
}
#endregion

public struct TimeUntil
{
    private float targetTime;
    public TimeUntil(float seconds) => targetTime = GameTime.CurrentTime + seconds;

    public static implicit operator TimeUntil(float seconds) => new TimeUntil(seconds);
    public static implicit operator bool(TimeUntil t) => GameTime.CurrentTime >= t.targetTime;
    public static implicit operator float(TimeUntil t) => Mathf.Max(0f, t.targetTime - GameTime.CurrentTime);
}

public struct TimeSince
{
    private float startTime;
    public TimeSince(float offset) => startTime = GameTime.CurrentTime - offset;

    public static implicit operator TimeSince(float offset) => new TimeSince(offset);
    public static implicit operator float(TimeSince t) => GameTime.CurrentTime - t.startTime;
}

public struct RealTimeUntil
{
    private float targetTime;
    public RealTimeUntil(float seconds) => targetTime = GameTime.UnscaledTime + seconds;

    public static implicit operator RealTimeUntil(float seconds) => new RealTimeUntil(seconds);
    public static implicit operator bool(RealTimeUntil t) => GameTime.UnscaledTime >= t.targetTime;
    public static implicit operator float(RealTimeUntil t) => Mathf.Max(0f, t.targetTime - GameTime.UnscaledTime);
}

public struct RealTimeSince
{
    private float startTime;
    public RealTimeSince(float offset) => startTime = GameTime.UnscaledTime - offset;

    public static implicit operator RealTimeSince(float offset) => new RealTimeSince(offset);
    public static implicit operator float(RealTimeSince t) => GameTime.UnscaledTime - t.startTime;
}