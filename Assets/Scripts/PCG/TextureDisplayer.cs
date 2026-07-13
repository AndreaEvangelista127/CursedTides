using UnityEngine;
using System.Collections;

public class TextureDisplayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshR;
    [SerializeField] private MeshRenderer _colorMeshR;
    [SerializeField] private MeshRenderer _terrainMesh;

    public void DisplayNoiseTexture(Texture2D texture)
    {
        if(_meshR == null || texture == null) return;

        _meshR.sharedMaterial.mainTexture = texture;

        // Divide by a factor to keep pixels compressed instead of spread out
        _meshR.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
    }

    public void DisplayColorTexture(Texture2D texture)
    {
        if (_colorMeshR == null || texture == null) return;
        _colorMeshR.sharedMaterial.mainTexture = texture;
    }

    public void DisplayTerrainTexture(Texture2D texture)
    {
        if(_terrainMesh == null || texture == null) return;
        _terrainMesh.sharedMaterial.mainTexture = texture;
    }

    
}
