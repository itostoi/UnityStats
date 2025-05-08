using UnityEngine;

public abstract class Duration {
    // public abstract bool Attach(GameObject target);
    // public abstract void Detach();
    public abstract float GetRemaining();
    public bool IsValid { get { return GetRemaining() > 0; } }
    public abstract void Update();
}

public class TimeDuration : Duration {
    public TimeDuration(float _duration)
    {
        timeRemaining = _duration;
    }
    private float timeRemaining;

    public override float GetRemaining()
    {
        return timeRemaining;
    }

    public override void Update()
    {
        timeRemaining -= Time.deltaTime;
    }
}

public class PermanentDuration : Duration {
    public override float GetRemaining()
    {
        return 1;
    }
    public override void Update() {}
}
 