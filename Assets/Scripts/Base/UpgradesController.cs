using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesController : MonoBehaviour
{
    public TMP_Text titleTxt;
    public TMP_Text descriptionTxt;
    public GameObject tipoRecursoImg;
    public TMP_Text costTxt;
    public GameObject activateBtn;
    public GameObject deactivateBtn;
    public GameObject[] upgradesOffBtns;
    public GameObject[] upgradesOnBtns;
    public GameObject upgradesInfoPanel;
    public GameObject[] offLines;
    public GameObject[] onLines;
    public GameObject rootShipBtn;
    private UnityEngine.Events.UnityAction buttonCallBack;

    private void Start()
    {
        updateUpgradesBtnsAndLines();
    }

    public void onClickSelectUpgrade(int upgradeId)
    {
        scaleSelectedUpgrade(upgradeId);
        showUpgradeInfo(upgradeId);
        updateUpgradesBtnsAndLines();
        FindObjectOfType<AudioManager>().Play("SeleccionarPlaneta");
    }

    private void scaleSelectedUpgrade(int upgradeId)
    {
        for(int i = 0; i < GameManager.shipUpgrades.Length; i++)
        {
            if(i == upgradeId)
            {
                if (GameManager.shipUpgrades[upgradeId])
                {
                    upgradesOnBtns[i].transform.localScale = new Vector2(1.5f, 1.5f);
                }
                else
                {
                    upgradesOffBtns[i].transform.localScale = new Vector2(1.5f, 1.5f);
                }
            }
            else
            {
                upgradesOnBtns[i].transform.localScale = new Vector2(1, 1);
                upgradesOffBtns[i].transform.localScale = new Vector2(1, 1);
            }
        }

        if(upgradeId != -1)
        {
            rootShipBtn.transform.localScale = new Vector2(1f, 1f);
        }
    }

    private void showUpgradeInfo(int upgradeId)
    {
        upgradesInfoPanel.SetActive(true);
        titleTxt.text = GameManager.shipUpgradesTitle[upgradeId];
        descriptionTxt.text = GameManager.shipUpgradesDescription[upgradeId];
        costTxt.text = GameManager.shipUpgradesCost[upgradeId].ToString();
        tipoRecursoImg.SetActive(true);

        if (buttonCallBack != null)
        {
            deactivateBtn.GetComponent<Button>().onClick.RemoveListener(buttonCallBack);
            activateBtn.GetComponent<Button>().onClick.RemoveListener(buttonCallBack);
        }

        if (GameManager.shipUpgrades[upgradeId])
        {
            activateBtn.SetActive(false);
            deactivateBtn.SetActive(true);
            buttonCallBack = () => sellUpgrade(upgradeId);
            deactivateBtn.GetComponent<Button>().onClick.AddListener(buttonCallBack);
        }
        else
        {
            activateBtn.SetActive(true);
            deactivateBtn.SetActive(false);
            buttonCallBack = () => buyUpgrade(upgradeId);
            activateBtn.GetComponent<Button>().onClick.AddListener(buttonCallBack);
        }
    }

    public void buyUpgrade(int upgradeId)
    {
        if(checkUpgradeRequirements(upgradeId) && GameManager.uranioTotal >= GameManager.shipUpgradesCost[upgradeId])
        {
            GameManager.uranioTotal -= GameManager.shipUpgradesCost[upgradeId];
            GetComponent<BaseResourcesPanel>().updateUranioText();
            GameManager.shipUpgrades[upgradeId] = true;
            onClickSelectUpgrade(upgradeId);
            FindObjectOfType<AudioManager>().Play("ActivarMejora");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("ErrorBtn");
        }
    }

    public void sellUpgrade(int upgradeId)
    {
        if (checkDeactivateRequirements(upgradeId))
        {
            GameManager.uranioTotal += GameManager.shipUpgradesCost[upgradeId];
            GetComponent<BaseResourcesPanel>().updateUranioText();
            GameManager.shipUpgrades[upgradeId] = false;
            onClickSelectUpgrade(upgradeId);
            FindObjectOfType<AudioManager>().Play("DesactivarMejora");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("ErrorBtn");
        }
    }

    private void updateUpgradesBtnsAndLines()
    {
        for( int i = 0; i < upgradesOffBtns.Length; i++)
        {
            upgradesOffBtns[i].SetActive(!GameManager.shipUpgrades[i]);
            upgradesOnBtns[i].SetActive(GameManager.shipUpgrades[i]);
            offLines[i].SetActive(!GameManager.shipUpgrades[i]);
            onLines[i].SetActive(GameManager.shipUpgrades[i]);
            if (checkUpgradeRequirements(i))
            {
                upgradesOffBtns[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                upgradesOnBtns[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                upgradesOffBtns[i].GetComponent<Image>().color = new Color32(40, 40, 40, 255);
                upgradesOnBtns[i].GetComponent<Image>().color = new Color32(40, 40, 40, 255);
            }
        }
    }

    private bool checkUpgradeRequirements(int upgradeId)
    {
        switch(upgradeId)
        {
            case 0: return true;
            case 1:
                if (!GameManager.shipUpgrades[0]) return false;
                if (GameManager.shipUpgrades[2]) return false;
                return true;
            case 2:
                if (!GameManager.shipUpgrades[0]) return false;
                if (GameManager.shipUpgrades[1]) return false;
                return true;
            default: return false; //Nunca debería ser default pero la función tiene que devolver algo
        }
    }

    private bool checkDeactivateRequirements(int upgradeId)
    {
        switch (upgradeId)
        {
            case 0:
                if (GameManager.shipUpgrades[1] || GameManager.shipUpgrades[2]) return false;
                return true;
            default: return true; //Si no llega una mejora con restricciones asumimos que se puede desactivar
        }
    }

    public void rootUpgrade()
    {
        updateUpgradesBtnsAndLines();
        upgradesInfoPanel.SetActive(true);
        titleTxt.text = "Nave Espacial";
        descriptionTxt.text = "Mejoras relacionadas con el funcionamiento de la nave";
        costTxt.text = "";
        tipoRecursoImg.SetActive(false);
        activateBtn.SetActive(false);
        deactivateBtn.SetActive(false);
        rootShipBtn.transform.localScale = new Vector2(1.25f, 1.25f);
        scaleSelectedUpgrade(-1);
    }
}
