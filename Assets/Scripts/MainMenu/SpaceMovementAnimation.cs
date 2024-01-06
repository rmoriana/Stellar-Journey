using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceMovementAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public float minSpeed;
    public float maxSpeed;
    public float minStartX;
    public float maxStartX;
    public float minScale;
    public float maxScale;
    public float generationSpeed;

    void Start()
    {
        InvokeRepeating("createStar", 0f, generationSpeed);
    }

    private void createStar()
    {
        GameObject newStar = Instantiate(Resources.Load("Prefabs/SmallStar"), new Vector3(Random.Range(minStartX, maxStartX), 10, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
        newStar.GetComponent<StarController>().speed = Random.Range(minSpeed, maxSpeed);
        float tmpScale = Random.Range(minScale,maxScale);
        newStar.transform.localScale = new Vector2(tmpScale, tmpScale);
        newStar.GetComponent<SpriteRenderer>().color = new Color32((byte)Random.Range(20,255), (byte)Random.Range(20, 255), (byte)Random.Range(20, 255), (byte)Random.Range(50, 255));
    }
}
