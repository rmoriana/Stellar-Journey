using UnityEngine;
using TMPro;

public class RecursosController : MonoBehaviour
{

    public TMP_Text textResources;
    private int currentResources;

    //Setea los recursos a 0 y actualiza el HUD
    void Start()
    {
        currentResources = 0;
        updateResourcesText();
    }

    //Añade una unidad de recurso y actualiza el HUD
    public void addResource()
    {
        currentResources++;
        updateResourcesText();
    }

    //Actualiza el contador de recursos
    private void updateResourcesText()
    {
        textResources.text = currentResources.ToString();
    }
}

