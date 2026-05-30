using UnityEngine;
using UnityEngine.Events;

public class FireDecay : MonoBehaviour
{
    [SerializeField] float decayTime;
    [SerializeField] float speed;
    [SerializeField] UnityEvent onDestroy;

    private float timeExisting = 0;

    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (timeExisting >= decayTime && !GetComponent<ParticleSystem>().isStopped)
        {
            GetComponent<ParticleSystem>().Stop();
            onDestroy.Invoke();
        } 
        
        if (timeExisting >= (decayTime + 5))
        {
            Destroy(gameObject);
        }
        
        timeExisting += Time.deltaTime;

        transform.position += transform.right * speed * Time.deltaTime;
    }
}
