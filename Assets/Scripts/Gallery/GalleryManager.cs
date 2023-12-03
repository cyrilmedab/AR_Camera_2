using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    private List<string> dbImageKeys = new();

    // Start is called before the first frame update
    void Start()
    {
        GetAllKeys(FirebaseManager.Instance.userDatabaseImages);
    }

    private async void GetAllKeys(DatabaseReference dbRef)
    {
        DataSnapshot imagesSnapshot = await FirebaseManager.Instance.GetDataSnapshot(dbRef);

        foreach (DataSnapshot image in imagesSnapshot.Children)
        {
            string key = image.Key.ToString();
            dbImageKeys.Add(key);
        }
    }

}
