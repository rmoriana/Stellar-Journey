using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{

    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public GameObject astralitaAvailableImage;
    public TMP_Text mejorAstralitaTxt;
    public TMP_Text mejorTiempoTxt;
    public TMP_Text costeAstralitaTxt;
    public GameObject unlockedPanel;
    public GameObject lockedPanel;
    public GameObject startGameBtn;
    public GameObject unlockBtn;
    public GameObject[] lockedPlanets;
    public GameObject[] unlockedPlanets;
    public GameObject[] planetSelectors;
    private UnityEngine.Events.UnityAction buttonCallBack;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i < GameManager.numLevels; i++)
        {
            if (GameManager.levelsState[i])
            {
                lockedPlanets[i].SetActive(false);
                unlockedPlanets[i].SetActive(true);
            }
            else
            {
                lockedPlanets[i].SetActive(true);
                unlockedPlanets[i].SetActive(false);
            }
        }

        unlockedPanel.SetActive(false);
        lockedPanel.SetActive(false);

        changeSelectedPlanet(-1);
        titleText.text = "";
        descriptionText.text = "";
    }

    public void clickOnUnlockedPlanet(int planetId)
    {
        unlockedPanel.SetActive(true);
        lockedPanel.SetActive(false);
        changeSelectedPlanet(planetId);

        if(GameManager.levelsMaxAstralita[planetId] == 0)
        {
            mejorAstralitaTxt.text = "-";
        }
        else
        {
            mejorAstralitaTxt.text = GameManager.levelsMaxAstralita[planetId].ToString();
        }

        if (GameManager.levelsLongestExpedition[planetId] == null)
        {
            mejorTiempoTxt.text = "-";
        }
        else
        {
            mejorTiempoTxt.text = GameManager.levelsLongestExpedition[planetId].ToString();
        }

        titleText.text = GameManager.levelsNames[planetId];

        if(buttonCallBack != null)
        {
            startGameBtn.GetComponent<Button>().onClick.RemoveListener(buttonCallBack);
        }

        buttonCallBack = () => startLevel(planetId);
        startGameBtn.GetComponent<Button>().onClick.AddListener(buttonCallBack);
    }

    public void clickOnLockedPlanet(int planetId)
    {
        unlockedPanel.SetActive(false);
        lockedPanel.SetActive(true);
        changeSelectedPlanet(planetId);

        titleText.text = GameManager.levelsNames[planetId];
        costeAstralitaTxt.text = GameManager.astralitaUnlockCost[planetId].ToString();

        if(GameManager.astralitaTotal >= GameManager.astralitaUnlockCost[planetId])
        {
            unlockBtn.GetComponent<Button>().interactable = true;
            if (buttonCallBack != null)
            {
                unlockBtn.GetComponent<Button>().onClick.RemoveListener(buttonCallBack);
            }

            buttonCallBack = () => unlockLevel(planetId);
            unlockBtn.GetComponent<Button>().onClick.AddListener(buttonCallBack);
        }
        else
        {
            unlockBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void startLevel(int planetId)
    {
        GameManager.currentLevel = planetId;
        SceneManager.LoadScene("Nivel-" + planetId);
    }

    public void unlockLevel(int planetId)
    {
        if(GameManager.astralitaTotal >= GameManager.astralitaUnlockCost[planetId])
        {
            GameManager.levelsState[planetId] = true;
            GameManager.astralitaTotal -= GameManager.astralitaUnlockCost[planetId];
            GetComponent<BaseResourcesPanel>().updateAstralitaText();
            lockedPlanets[planetId].SetActive(false);
            unlockedPlanets[planetId].SetActive(true);
            clickOnUnlockedPlanet(planetId);
        }
    }

    private void changeSelectedPlanet(int planetId)
    {
        for(int i = 0; i < planetSelectors.Length; i++)
        {
            if(i == planetId)
            {
                planetSelectors[i].SetActive(true);
            }
            else
            {
                planetSelectors[i].SetActive(false);
            }
        }
    }
}
