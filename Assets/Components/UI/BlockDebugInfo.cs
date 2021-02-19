using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockDebugInfo : MonoBehaviour
{
    [SerializeField] private Text infoText;
    private bool _shown = false;

    public void UpdateText (string text)
    {
        infoText.text = text;
    }

    public void ToggleShown ()
    {
        _shown = !_shown;
        GetComponent<Image>().enabled = _shown;
        infoText.GetComponent<Text>().enabled = _shown;
    }
}
