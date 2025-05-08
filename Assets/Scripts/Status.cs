using UnityEngine;

public class Status : MonoBehaviour
{
    public Duration duration;
    public Effect effect;

    void Start()
    {
        // Debug.Log($"Effect exists? {effect != null}");
        effect.Attach(gameObject);
    }
    void Update()
    {
        duration.Update();
        if (duration.GetRemaining() <= 0 )
        {
            Destroy(this);
        }
    }
    void OnDestroy()
    {
        effect.Detach();
    }
}
