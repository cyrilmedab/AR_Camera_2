using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GalleryPolaroid : MonoBehaviour
{
    public string dbHash;

    [SerializeField]
    private Image photoImage;

    [SerializeField]
    private TextMeshProUGUI photoName;

    public void SetNameAndImage((string, byte[]) photo)
    {
        var (name, image) = photo;
        // Sliced to remove the ".png" from the file name
        photoName.text = name[0..^4];

        SetImage(image);
    }

    private void SetImage(byte[] image)
    {
        Debug.Log(image);
        Texture2D imgTexture = new Texture2D(1, 1);
        imgTexture.LoadImage(image);

        Sprite imgSprite = Sprite.Create(imgTexture, new Rect(0f, 0f, imgTexture.width, imgTexture.height)
            , new Vector2(0.5f, 0.5f), 100.0f);
        photoImage.sprite = imgSprite;
    }

    public async void ChangeName()
    {
        // Prevents duplicate polaroids or new ones being created while we add and delete to the database
        FirebaseManager.Instance.userDatabaseImages.ChildAdded -= GalleryManager.Instance.AddKey;

        string newName = photoName.text;

        // Delete oldvalues from storage and database
        await FirebaseManager.Instance.DeleteStoredData(dbHash);

        // Update value in Storage and Database
        dbHash = await FirebaseManager.Instance.UploadImageToStorage(photoImage.sprite.texture, newName);
        
        //inputField.text = "";

        // Reconnects the listener after we're done modifying the database and current polaroid
        FirebaseManager.Instance.userDatabaseImages.ChildAdded += GalleryManager.Instance.AddKey;
    }
}
