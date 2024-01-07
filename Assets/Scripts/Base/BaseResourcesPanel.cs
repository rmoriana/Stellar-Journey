using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseResourcesPanel : MonoBehaviour
{
    public TMP_Text astralitaText;
    public TMP_Text uranioText;
    // Start is called before the first frame update
    void Start()
    {
        updateAstralitaText();
        updateUranioText();
    }

    public void updateAstralitaText()
    {
        astralitaText.text = GameManager.astralitaTotal.ToString();
    }

    public void updateUranioText()
    {
        uranioText.text = GameManager.uranioTotal.ToString();
    }
}
