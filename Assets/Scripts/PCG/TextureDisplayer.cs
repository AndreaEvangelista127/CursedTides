using UnityEngine;

public class TextureDisplayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshR;

    public void DisplayTexture(Texture2D texture)
    {
        if(_meshR == null || texture == null) return;

        _meshR.sharedMaterial.mainTexture = texture; 

        //_meshR.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
