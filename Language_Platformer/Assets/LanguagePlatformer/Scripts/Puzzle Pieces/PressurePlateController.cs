using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PressurePlateController : Activator
{
    
    public PressurePlate pressurePlate;
    private Collider2D coll;
    private SpriteRenderer spriteRenderer;
    private Dictionary<string, Sprite> spritePairs = new Dictionary<string, Sprite>();
    private bool permanent;
    private float loadNeeeded;
    private float fallTime;
    private float riseTime;
    private bool active = false;
    private bool goingDown = false;
    private bool goingUp = true;
    private List<int> objectsOn = new List<int>();
    private float totalMass;
    private float upSpot;
    private float downSpot;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        permanent = pressurePlate.permanent;
        loadNeeeded = pressurePlate.loadNec;
        fallTime = pressurePlate.fallTime;
        riseTime = pressurePlate.riseTime;
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < pressurePlate.spriteKeys.Length; i++)
        {
            spritePairs.Add(pressurePlate.spriteKeys[i], pressurePlate.spriteValues[i]);
        }
        upSpot = transform.position.y;
        downSpot = transform.position.y - 0.25f;
    }


    void Update()
    {
        
    }

    void FixedUpdate()
    {

        if (totalMass >= loadNeeeded && !goingDown)
        {
            
            goingUp = false;
            goingDown = true;
            StopCoroutine(UpPlate());
            StartCoroutine(DownPlate());
        }
        else if (totalMass < loadNeeeded && !goingUp && !permanent)
        {

            active = false;
            goingDown = false;
            goingUp = true;
            StopCoroutine(DownPlate());
            StartCoroutine(UpPlate());
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 hitNormal = other.GetContact(0).normal; //get the collision normal
        if (hitNormal.y < -0.5f)
        {
            print("hooray, obj added");
            objectsOn.Add((int)other.rigidbody.mass);
            totalMass += (int)other.rigidbody.mass;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (objectsOn.Contains((int)other.rigidbody.mass))
        {
            objectsOn.Remove((int)other.rigidbody.mass);
            totalMass -= (int)other.rigidbody.mass;
            print("bye bye");
        }
    }

    IEnumerator UpPlate()
    {
        float incrementer = 0f;
        float timeRatio = Mathf.InverseLerp(downSpot, upSpot, transform.position.y);

        while (incrementer < 0.1f)
        {
            incrementer += Time.deltaTime;
            yield return null;
        }
        incrementer = 0f;
        if (totalMass >= loadNeeeded)
            yield break;

        while (incrementer < riseTime * (1 - timeRatio))
        {

            incrementer += Time.deltaTime;
            float normalizedTime = incrementer / riseTime;
            float currValue = Mathf.Lerp(transform.position.y, upSpot, normalizedTime);
            transform.position = new Vector3(transform.position.x, currValue, transform.position.z);
            yield return null;
        }
        yield return null;
    }
    IEnumerator DownPlate()
    {
        float incrementer = 0f;
        float timeRatio = Mathf.InverseLerp(upSpot, downSpot, transform.position.y);
        
        while (incrementer < 0.1f)
        {
            incrementer += Time.deltaTime;
            yield return null;
        }
        incrementer = 0f;
        if (totalMass < loadNeeeded)
            yield break;

        while (incrementer < fallTime * (1 - timeRatio))
        {

            incrementer += Time.deltaTime;
            float normalizedTime = incrementer / fallTime;
            float currValue = Mathf.Lerp(transform.position.y, downSpot, normalizedTime);
            transform.position = new Vector3(transform.position.x, currValue, transform.position.z);
            yield return null;
        }
        active = true;
        yield return null;
    }


    public override bool GetActive()
    {
        return active;
    }


}
