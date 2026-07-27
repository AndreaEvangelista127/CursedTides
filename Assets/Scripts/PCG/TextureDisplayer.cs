using UnityEngine;
using System.Collections;

public class TextureDisplayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _baseMeshR;
    [SerializeField] private MeshRenderer _falloffMeshR;
    [SerializeField] private MeshRenderer _colorMeshR;
    [SerializeField] private MeshRenderer _terrainMesh;


    public void DisplayBaseNoiseTexture(Texture2D texture)
    {
        if (_baseMeshR == null || texture == null) return;
        _baseMeshR.sharedMaterial.mainTexture = texture;
    }

    public void DisplayFalloffTexture(Texture2D texture)
    {
        if(_falloffMeshR == null || texture == null) return;

        _falloffMeshR.sharedMaterial.mainTexture = texture;

        // Divide by a factor to keep pixels compressed instead of spread out
        _falloffMeshR.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
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
