using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamDisplay : MonoBehaviour
{
    public bool displayEnabled = true;
    public bool mirror = true;
    public UnityEngine.UI.Image targetImage;
    WebCamTexture camTexture;
    void Start()
    {
        camTexture = new WebCamTexture();
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        camTexture.Play();
    }

    void Update()
    {
        if (displayEnabled)
        {
            DisplayFrame();
        }
    }

    public void DisplayFrame()
    {
        if (targetImage.sprite == null || 
            targetImage.sprite.texture.width != camTexture.width || 
            targetImage.sprite.texture.height != camTexture.height)
        {
            targetImage.sprite = Sprite.Create(new Texture2D(camTexture.width, camTexture.height), new Rect(0, 0, camTexture.width, camTexture.height), Vector2.zero);
        }

        Color[] pix = camTexture.GetPixels();

        // Reverse each row of pixels, if necessary.
        if (mirror)
        {
            for (int i = 0; i < camTexture.height; i++)
            {
                System.Array.Reverse(pix, i * camTexture.width, camTexture.width);
            }
        }

        targetImage.sprite.texture.SetPixels(pix);
        targetImage.sprite.texture.Apply();
    }
}
