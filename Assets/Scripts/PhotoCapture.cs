 using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Display")]
    [SerializeField]
    private Image photoDisplayArea;
    [SerializeField]
    private GameObject polaroidFrame;
    [SerializeField]
    private float photoDevelopTime;

    [Header("Flash Effect")]
    [SerializeField]
    private Image cameraFlash;
    [SerializeField]
    private float flashTime;

    [Header("Audio")]
    [SerializeField]
    private AudioSource cameraShutterAudio;

    [Header("NamingTextField")]
    [SerializeField]
    private TMP_InputField fileNameText;

    private Texture2D _screenCapture;
    private bool _savingPhoto;


    void Start()
    {
        // Gives us an empty Texture2D to start with
        _screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    // Publicly accessible function that determines whether we should be capturing a new photo or removing the previous one
    public void CapturePhoto() { if (!_savingPhoto) StartCapture(); }


    // Begins the process of capturing the screen and displaying a polaroid of it
    private async void StartCapture()
    {
        // Starts the Audio sound effect for the camera
        cameraShutterAudio.Play();

        // Starts the first half of the camera flash image, making it solid white
        cameraFlash.gameObject.SetActive(true);
        await Utils.FadeSprite(cameraFlash, 1f, flashTime);

        // turned off briefly so we don't screen capture the flash (turned back on in the CapturePhotoCoroutine)
        cameraFlash.gameObject.SetActive(false);
        // At the peak of the flash, starts the process of capturing and displaying the polaroid
        StartCoroutine(CapturePhotoCoroutine());
        // Sets this to zero in advance so it doesn't appear to early
        Utils.SetSpriteAlpha(photoDisplayArea, 0f);


        // Starts the last half of the camera flash, making it transparent before being set inactive
        await Utils.FadeSprite(cameraFlash, 0f, flashTime * 1.5f);
        cameraFlash.gameObject.SetActive(false);

        // Finally, we want to turn on the photo display area and fade it in, to create the effect of film developing
        photoDisplayArea.gameObject.SetActive(true);
        await Utils.FadeSprite(photoDisplayArea, 1f, photoDevelopTime);
    }

    // Coroutine to capture the current image of the screen and store it as a texture to be used for the polaroid effect
    private IEnumerator CapturePhotoCoroutine()
    {
        _savingPhoto = true;

        // Makes sure the Frame is finished before taking capture
        yield return new WaitForEndOfFrame();

        // Reads all the pixels on the screen and stores it in our screenCapture
        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);
        _screenCapture.ReadPixels(regionToRead, 0, 0, false);

        // Turns the flash image back on after the screen capture is done
        cameraFlash.gameObject.SetActive(true);

        // Applies the texture to the screen
        _screenCapture.Apply();
        DisplayPhoto();
    }

    private void DisplayPhoto()
    {
        // Makes a sprite from our screen capture and puts it onto the polaroid UI
        Sprite photoSprite = Sprite.Create(_screenCapture, new Rect(0f, 0f, Screen.width, Screen.height),
            new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;

        // Makes the UI visible and readies the input field for file naming
        polaroidFrame.SetActive(true);
        fileNameText.enabled = true;
    }

    // Resets The UI and removes the polaroid
    public void RemovePhoto()
    {
        // Removes the polaroid background, the photo itself, deactivates the input field,
        // and sets the bool to allow us to take a new photo again
        _savingPhoto = false;
        polaroidFrame.SetActive(false);
        photoDisplayArea.gameObject.SetActive(false);
        fileNameText.text = "";
        fileNameText.enabled = false;
    }

}
