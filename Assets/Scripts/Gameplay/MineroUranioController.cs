using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineroUranioController : MonoBehaviour
{

    public float miningSpeed;
    private GameObject spaceship;
    private GameObject bottomRightPanel;
    private float miningCooldown;
    // Start is called before the first frame update
    void Start()
    {
        spaceship = GameObject.Find("Spaceship_P");
        bottomRightPanel = GameObject.Find("BottomRightPanel");
        FindObjectOfType<AudioManager>().PlayLoop("MineroUranioDriller");
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<CombatController>().isBeingDeployed)
        {
            return;
        }

        if (miningCooldown >= miningSpeed)
        {
            spaceship.GetComponent<Spaceship_C>().deployMineral(1);
            FindObjectOfType<AudioManager>().Play("UranioGain");
            miningCooldown = 0;
        }
        else
        {
            miningCooldown += Time.deltaTime;
        }

        bottomRightPanel.GetComponent<RecursosController>().updateUranioTime(miningCooldown);
    }
}
