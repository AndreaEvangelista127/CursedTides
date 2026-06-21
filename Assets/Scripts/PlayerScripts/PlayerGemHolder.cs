using UnityEngine;
using UnityEngine.UI;

public class PlayerGemHolder : MonoBehaviour
{

    [SerializeField] private GameObject _gemIconUI;    

    public bool HasGem { get; private set; } = false;
    public GemType? HeldGemType { get; private set; } = null;

    public void PickUp(GemType gemType, Sprite icon)
    {
        HasGem = true;                    
        HeldGemType = gemType;
        _gemIconUI.GetComponent<Image>().sprite = icon;
        _gemIconUI.SetActive(true);       
    }

    public void PlaceGem()
    {
        HasGem = false;                  
        HeldGemType = null;               
        _gemIconUI.SetActive(false);     
    }
}

