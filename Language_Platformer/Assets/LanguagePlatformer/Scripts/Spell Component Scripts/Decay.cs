using UnityEngine;
using UnityEngine.Events;

public class Decay : MonoBehaviour
{
    [SerializeField] float decayTime;
    [SerializeField] float speed;
    [SerializeField] float speedDecay = 0.0f;
    [SerializeField] UnityEvent onDestroy;
    [SerializeField] bool identityRotation = false;

    private float timeExisting = 0;
    private bool spellOn = true;

    void Start()
    {
        if (identityRotation)
            transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (timeExisting >= decayTime && !GetComponent<ParticleSystem>().isStopped)
        {
            if (GetComponent<Collider2D>())
                GetComponent<Collider2D>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
            onDestroy.Invoke();
        } 
        
        if (timeExisting >= decayTime + GetComponent<ParticleSystem>().main.duration)
        {
            spellOn = false;
            Destroy(gameObject);
        }
        
        timeExisting += Time.deltaTime;

        transform.position += transform.right * -speed * Time.deltaTime;
        speed = Mathf.Clamp(speed - (speedDecay * Time.deltaTime), 0, Mathf.Infinity);
    }

    public bool SpellActive()
    {
        return spellOn;
    }
}
