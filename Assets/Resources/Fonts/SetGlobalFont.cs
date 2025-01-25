using TMPro;
using UnityEngine;

public class SetGlobalFont : MonoBehaviour
{
    public TMP_FontAsset _font;

    private void Start()
    {
        // find all active and inactive text mesh pro objects and set the font 
        TMP_Text[] textMeshPros = FindObjectsOfType<TMP_Text>();
        foreach (TMP_Text textMeshPro in textMeshPros)
        {
            textMeshPro.font = _font;
        }
    }
}