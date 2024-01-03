using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseOptionSelection : MonoBehaviour
{
    public GameObject stellarMapBtn;
    public GameObject upgradesBtn;
    public GameObject upgradesPanel;
    public GameObject lvlUnlockedInfoPanel;
    public GameObject lvlLockedInfoPanel;
    public GameObject upgradesInfoPanel;
    public GameObject tituloTxt;
    public GameObject recursosTxt;

    private const int DEFAULT = 0;
    private const int PRESSED = 1;

    public Sprite[] stellarMapSprites;
    public Sprite[] upgradesBtnSprites;

    // Start is called before the first frame update
    void Start()
    {
        onClickChangeMenu(0);
    }

    public void onClickChangeMenu(int btnId)
    {
        switch (btnId)
        {
            case 0:
                upgradesPanel.SetActive(false);
                stellarMapBtn.GetComponent<Image>().sprite = stellarMapSprites[PRESSED];
                upgradesBtn.GetComponent<Image>().sprite = stellarMapSprites[DEFAULT];
                upgradesInfoPanel.SetActive(false);
                tituloTxt.SetActive(true);
                recursosTxt.SetActive(true);
                break;
            case 1:
                upgradesPanel.SetActive(true);
                stellarMapBtn.GetComponent<Image>().sprite = stellarMapSprites[DEFAULT];
                upgradesBtn.GetComponent<Image>().sprite = stellarMapSprites[PRESSED];
                lvlLockedInfoPanel.SetActive(false);
                lvlUnlockedInfoPanel.SetActive(false);
                tituloTxt.SetActive(false);
                recursosTxt.SetActive(false);
                break;
        }
    }
}
