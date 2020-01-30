using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public static Queue<GameObject> Obstacles;

    public int PoolSize;
    public float Speed;
    public float Smooth;
    public Vector2 WidthRange;
    public Vector2 HeightRange;
    public float Delta;
    public int D;
    public Transform ObstaclesContainer;
    public GameObject Obstacle;

    private float time;
    private Vector3 startPos;
    private GameObject top;
    private GameObject bottom;
    private float topHeight;
    private float topWidth;
    private float bottomHeight;
    private float bottomWidth;

    private float topInterval
    {
        get => (topWidth - Smooth / Speed) / Speed;
    }

    private float bottomInterval
    {
        get => (bottomWidth - Smooth / Speed) / Speed;
    }

    private Vector3 topScale
    {
        get => new Vector3(topWidth, topHeight, 1);
    }

    private Vector3 bottomScale
    {
        get => new Vector3(bottomWidth, bottomHeight, 1);
    }

    void Awake()
    {
        startPos = new Vector3(15f, 0f, 0f);
        FillPool();
    }

    void Start()
    {
        StartCoroutine(topRandGen());
        StartCoroutine(bottomRandGen());
        // StartCoroutine(generator());
    }

    private void FillPool()
    {
        Obstacles = new Queue<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject clone = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
            clone.SetActive(false);
            Obstacles.Enqueue(clone);
        }
    }

    private GameObject GetObstacle()
    {
        GameObject clone = Obstacles.Dequeue();
        clone.transform.position = startPos;
        updateSpeed();
        return clone;
    }

    private void updateSpeed()
    {
        Mover.Speed = Speed;
    }

    private void updateTopTransform()
    {
        top.transform.localScale = topScale;
        top.transform.position = new Vector3(top.transform.position.x, 5 - top.transform.localScale.y / 2, 0f);
    }

    private void updateBottomTransform()
    {
        bottom.transform.localScale = bottomScale;
        bottom.transform.position = new Vector3(bottom.transform.position.x, -5 + bottom.transform.localScale.y / 2, 0f);
    }

    private IEnumerator topRandGen()
    {
        topWidth = WidthRange.x;

        while (true)
        {
            top = GetObstacle();
            topHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateTopTransform();
            yield return new WaitForSeconds(topInterval);
            top.SetActive(true);
        }
    }

    private IEnumerator bottomRandGen()
    {
        bottomWidth = WidthRange.x;

        while (true)
        {
            bottom = GetObstacle();
            bottomHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateBottomTransform();
            yield return new WaitForSeconds(bottomInterval);
            bottom.SetActive(true);
        }
    }

    private IEnumerator generator()
    {
        Speed = 9;
        HeightRange = new Vector2(0.5f, 2);
        WidthRange = new Vector2(3, 3);
        D = 1;
        Smooth = 2;
        Delta = 0.5f;

        while (true)
        {
            WidthRange = new Vector2(3, 3);
            Delta = 0.5f;

            yield return StartCoroutine(gen1());
            yield return StartCoroutine(gen2());

            if (HeightRange.y < 4f)
            {
                HeightRange.x += 0.5f;
                HeightRange.y += 0.5f;
            }

            if (Speed < 15)
            {
                Speed += 1f;
                Smooth += 0.5f;
            }

            if (D < 7)
            {
                D++;
            }
        }
    }

    private IEnumerator gen1()
    {
        float height = HeightRange.x;
        float width = WidthRange.x;
        bool h = true;
        int T = (int)((HeightRange.y - HeightRange.x) / Delta) * 2;
        int t = 0;

        while (t < D * T)
        {
            top = GetObstacle();
            bottom = GetObstacle();

            if (h)
                height = Mathf.MoveTowards(height, HeightRange.y, Delta);
            else
                height = Mathf.MoveTowards(height, HeightRange.x, Delta);

            if (height <= HeightRange.x)
                h = true;
            else if (height >= HeightRange.y)
                h = false;

            width = Random.Range(WidthRange.x, WidthRange.y);

            topHeight = bottomHeight = height;
            topWidth = bottomWidth = width;
            updateTopTransform();
            updateBottomTransform();

            yield return new WaitForSeconds(topInterval);
            t++;

            top.SetActive(true);
            bottom.SetActive(true);
        }
    }

    private IEnumerator gen2()
    {
        topHeight = HeightRange.x;
        bottomHeight = HeightRange.y;

        float width = WidthRange.x;
        bool h = true;
        int T = (int)((HeightRange.y - HeightRange.x) / Delta) * 2;
        int t = 0;

        while (t < D * T)
        {
            top = GetObstacle();
            bottom = GetObstacle();

            if (h)
            {
                topHeight = Mathf.MoveTowards(topHeight, HeightRange.y, Delta);
                bottomHeight = Mathf.MoveTowards(bottomHeight, HeightRange.x, Delta);
            }
            else
            {
                topHeight = Mathf.MoveTowards(topHeight, HeightRange.x, Delta);
                bottomHeight = Mathf.MoveTowards(bottomHeight, HeightRange.y, Delta);
            }

            if (topHeight <= HeightRange.x)
            {
                h = true;
            }
            else if (topHeight >= HeightRange.y)
            {
                h = false;
            }

            width = Random.Range(WidthRange.x, WidthRange.y);

            topWidth = bottomWidth = width;
            updateTopTransform();
            updateBottomTransform();

            yield return new WaitForSeconds(topInterval);
            t++;

            top.SetActive(true);
            bottom.SetActive(true);
        }
    }
}
