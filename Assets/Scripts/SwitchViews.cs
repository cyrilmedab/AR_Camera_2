using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchViews : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCanvas;
    [SerializeField]
    private GameObject galleryCanvas;

    // Publicly-accessible method to access either TurnOnMain or TurnOnGallery depending on bool
    public void SwitchView(bool view)
    {
        if (view) StartCoroutine(TurnOnGallery());
        else StartCoroutine(TurnOnMain());
    }

    // Turns off the Gallery UI and turns on the Main UI
    private IEnumerator TurnOnMain()
    {
        // Prevents an Exception from being thrown from the input system trying to detect touch on a button that will no longer exist
        yield return new WaitForEndOfFrame(); 

        galleryCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    // Turns off the Main UI and turns on the Gallery UI
    private IEnumerator TurnOnGallery()
    {
        // Prevents an Exception from being thrown from the input system trying to detect touch on a button that will no longer exist
        yield return new WaitForEndOfFrame();

        mainCanvas.SetActive(false);
        galleryCanvas.SetActive(true);
    }
}
