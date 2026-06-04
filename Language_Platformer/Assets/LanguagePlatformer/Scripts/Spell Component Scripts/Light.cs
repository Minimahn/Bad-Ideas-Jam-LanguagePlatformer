using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class Light : MonoBehaviour
{
    private Transform parent;
    private PlayerVariables playerVariables;
    private float xCenter, yCenter;
    private static float STAR_MOVE_TIME = 1f;


    void Start()
    {
        parent = transform.parent;
        playerVariables = parent.GetComponent<PlayerVariables>(); //only works for plr.
        playerVariables.setGlow(true);

        xCenter = transform.localPosition.x;
        yCenter = transform.localPosition.y;
        

        StartCoroutine(GetToPoint());
    }

    IEnumerator GetToPoint()
    {
        float xStart = transform.localPosition.x;
        float yStart = transform.localPosition.y;

        float xRand =  Random.Range(-1f, 1f);
        float yRand = Random.Range(-0.2f, 0.2f);
        float yVar = Random.Range(-0.1f, 0.1f);
        Vector3 midpointCurve = new Vector3(Mathf.Lerp(xCenter, xCenter + xRand, 0.5f), Mathf.Lerp(yCenter, yCenter + yRand + yVar, 0.5f), transform.localPosition.z);
        
        Vector3 endpoint = new Vector3(xCenter + xRand, yCenter + yRand, transform.localPosition.z);
        float halfTime = STAR_MOVE_TIME / 2f;
        float incrementer = 0f;

        while (incrementer < halfTime)
        {
            incrementer += Time.deltaTime;
            float ratio = Mathf.Clamp01(incrementer / halfTime);
            float lerpX = Mathf.Lerp(xStart, midpointCurve.x, ratio);
            float lerpY = Mathf.Lerp(yStart, midpointCurve.y, ratio);

            transform.localPosition = new Vector3(lerpX, lerpY, transform.localPosition.z);

            yield return null;
        }

        float xNow = transform.localPosition.x;
        float yNow = transform.localPosition.y;
        incrementer = 0f;

        while (incrementer < halfTime)
        {
            incrementer += Time.deltaTime;
            float ratio = Mathf.Clamp01(incrementer / halfTime);
            float lerpX = Mathf.Lerp(xNow, endpoint.x, ratio);
            float lerpY = Mathf.Lerp(yNow, endpoint.y, ratio);

            transform.localPosition = new Vector3(lerpX, lerpY, transform.localPosition.z);

            yield return null;
        }

        StartCoroutine(GetToPoint());
    }


    void OnDestroy()
    {
        playerVariables.setGlow(false);
    }

}
