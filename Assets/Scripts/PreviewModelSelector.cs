using UnityEngine;

public class PreviewModelSelector : MonoBehaviour
{
    [SerializeField] private ModelData[] _modelData;

    private GameObject _selectedSkin;


    public void SelectSkin(int skinIndex)
    {
        if (_selectedSkin != null) 
            _selectedSkin.SetActive(false);

        
        _selectedSkin = _modelData[skinIndex].Model;
        _selectedSkin.SetActive(true);

    }


    public string[] GetSkinNames()
    {
        string[] skinNames = null;

        if(_modelData.Length != 0)
        {
            skinNames = new string[_modelData.Length];
            for (int i = 0; i < _modelData.Length; i++)
            {
                skinNames[i] = _modelData[i].ModelName;
            }
        }

        return skinNames;
    }
}
