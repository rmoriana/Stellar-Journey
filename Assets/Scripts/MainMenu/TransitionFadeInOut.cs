using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionFadeInOut : MonoBehaviour
{
    public IEnumerator FadeInOutEffect(bool fadeToBlack, float fadeSpeed = 4)
    {
        Color objectColor = GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            while (GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                GetComponent<Image>().color = objectColor;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
