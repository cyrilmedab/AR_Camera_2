using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    List<string> dbImageKeys = new();

    // Start is called before the first frame update
    void Start()
    {
        GetAllKeys();
    }

    private async void GetAllKeys()
    {
        DataSnapshot imagesSnapshot = await FirebaseManager.Instance.GetDataSnapshot(FirebaseManager.Instance.userDatabaseImages);

        foreach (DataSnapshot image in imagesSnapshot.Children)
        {
            Debug.Log(image.Key);
        }
      
    }

    
}
