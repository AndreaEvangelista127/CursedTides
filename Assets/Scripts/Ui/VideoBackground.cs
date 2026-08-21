using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoBackground : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _rawImage;

    private void Start()
    {
        if( _videoPlayer != null && _rawImage != null)
        {
            // Create render texture
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 0);
            _videoPlayer.targetTexture = renderTexture;
            _rawImage.texture = renderTexture;

            _videoPlayer.isLooping = true;
            _videoPlayer.Play();
        }
    }
}
