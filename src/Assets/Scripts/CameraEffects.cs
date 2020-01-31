using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public Vector2 RotateRange;
    public float Delta;
    public float Delay;
    public float Target;

    private float rot;
    private bool r;
    private float target;

    void Start()
    {
        StartCoroutine(rotation());
    }

    void LateUpdate()
    {
        // rot1();
    }

    void rot1()
    {
        if (r)
            rot = Mathf.MoveTowards(rot, RotateRange.y, Time.deltaTime * Delta);
        else
            rot = Mathf.MoveTowards(rot, RotateRange.x, Time.deltaTime * Delta);

        if (rot <= RotateRange.x)
            r = true;
        else if (rot >= RotateRange.y)
            r = false;

        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    IEnumerator rot2()
    {
        float t = 0;
        yield return new WaitForSeconds(Delay);
        while (t < 2)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, target), 0.05f);
            t += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    IEnumerator rotation()
    {
        while (true)
        {
            target = Target;
            yield return StartCoroutine(rot2());
            target = 0;
            yield return StartCoroutine(rot2());
            target = -Target;
            yield return StartCoroutine(rot2());
            target = 0;
            yield return StartCoroutine(rot2());
        }
    }
}