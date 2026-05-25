using UnityEngine;
using UnityEngine.Events;

public class MoveForwards : MonoBehaviour
{
    [SerializeField] float decayTime;
    [SerializeField] float speed;
    [SerializeField] UnityEvent onDestroy;

    private float timeExisting = 0;

    // Update is called once per frame
    void Update()
    {
        if (timeExisting >= decayTime)
        {
            onDestroy.Invoke();
            Destroy(gameObject);
        }

        timeExisting += Time.deltaTime;

        transform.position += transform.right * speed * Time.deltaTime;
    }
}
