using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryPolaroid : MonoBehaviour
{
    [SerializeField]
    private Image photoImage;

    [SerializeField]
    private TextMeshProUGUI photoName;

    public void SetNameAndImage((string, byte[]) photo)
    {
        var (name, image) = photo;
        photoName.text = name;

        SetImage(image);
    }

    private void SetImage(byte[] image)
    {
        Texture2D imgTexture = new Texture2D(1, 1);
        imgTexture.LoadImage(image);

        Sprite imgSprite = Sprite.Create(imgTexture, new Rect(0f, 0f, Screen.width, Screen.height)
            , new Vector2(0.5f, 0.5f), 100.0f);
        photoImage.sprite = imgSprite;
    }
}
