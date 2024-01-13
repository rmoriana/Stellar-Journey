using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecursosController : MonoBehaviour
{

    public TMP_Text astralitaQuantityText;
    public TMP_Text uranioQuantityText;
    public GameObject uranioProgressBg;
    public Image uranioProgressBar;
    public GameObject uranioImage;
    private int currentAstralita, currentUranio;

    //TODO: Actualizar este script para que coja los datos que tiene la nave y así evitar duplicados.
    //Setea los recursos a 0 y actualiza el HUD
    void Start()
    {
        currentAstralita = 0;
        currentUranio = 0;
        if (GameManager.levelsResourcesAvailable[GameManager.currentLevel] == 1)
        {
            uranioImage.SetActive(true);
            uranioProgressBg.SetActive(true);
        }
        else
        {
            uranioImage.SetActive (false);
            uranioQuantityText.text = "";
            uranioProgressBg.SetActive(false);
        }
        updateResourcesText();
    }

    //Añade una unidad de recurso y actualiza el HUD
    public void addResource(int type)
    {
        switch (type)
        {
            case 0: currentAstralita ++; break;
            case 1: currentUranio++; break;
        }
        updateResourcesText();
    }

    //Actualiza el contador de recursos
    private void updateResourcesText()
    {
        astralitaQuantityText.text = currentAstralita.ToString();
        if (GameManager.levelsResourcesAvailable[GameManager.currentLevel] == 1)
        {
            uranioQuantityText.text = currentUranio.ToString();
        }
    }

    public void updateUranioTime(float time)
    {
        //TODO: Eliminar hardcode (poniendo el tiempo en GameManager)
        uranioProgressBar.fillAmount = time / 120;
    }
}

