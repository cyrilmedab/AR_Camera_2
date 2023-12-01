using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class RecordButton : MonoBehaviour
{
    [SerializeField] 
    private Sprite startUI;
    [SerializeField]
    private Sprite stopUI;
    [SerializeField]
    private float fadeTime = 1f;

    private Image _buttonImage;


    // Start is called before the first frame update
    void Start()
    {
        _buttonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestClick()
    {
        SwapSprite();
        Debug.Log("Button Clicked");
    }

    public async void SwapSprite()
    {
        await FadeOutButton();
        await FadeInButton();
    }

    public async Task<Task> FadeOutButton()
    {
        await Utils.FadeSprite(_buttonImage, 0f, fadeTime);
        return Task.CompletedTask;
    }

    public async Task<Task> FadeInButton()
    {
        if (_buttonImage.sprite == startUI) _buttonImage.sprite = stopUI;
        else _buttonImage.sprite = startUI;

        await Utils.FadeSprite(_buttonImage, 1f, fadeTime);
        return Task.CompletedTask;
    }
}
