using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GalleryPolaroid : MonoBehaviour
{
    public string dbHash;

    [SerializeField]
    private Image photoImageDisplay;

    [SerializeField]
    private TMP_InputField photoNameDisplay;

    public string photoName;
    public byte[] photoImage;

    public void SetNameAndImage((string, byte[]) photo)
    {
        var (full_name, image) = photo;

        // Sliced to remove the ".png" from the file name
        photoName = full_name[0..^4];
        photoNameDisplay.text = photoName;

        SetImage();
    }

    private void SetImage()
    {
        Debug.Log(photoImage);
        Texture2D imgTexture = new Texture2D(1, 1);
        ImageConversion.LoadImage(imgTexture, photoImage);

        Sprite imgSprite = Sprite.Create(imgTexture, new Rect(0f, 0f, imgTexture.width, imgTexture.height)
            , new Vector2(0.5f, 0.5f), 100.0f);
        photoImageDisplay.sprite = imgSprite;
    }

    public async void ChangeName()
    {
        // Prevents duplicate polaroids or new ones being created while we add and delete to the database
        GalleryManager.Instance.changingName = true;

        // Delete oldvalues from storage and database
        await FirebaseManager.Instance.DeleteStoredData(dbHash);

        // Update value in Storage and Database
        photoName = photoNameDisplay.text;
        dbHash = await FirebaseManager.Instance.UploadImageToStorage(photoImageDisplay.sprite.texture, photoName);

        //inputField.text = "";

        // arbitrary Delay because there was an issue with the changingName being set to false too quickly.
        await Task.Delay(1000);
        // Reenables the polaroid-creation process for newly-added data points
        GalleryManager.Instance.changingName = false;
        //Destroy(this.gameObject);
    }
}
