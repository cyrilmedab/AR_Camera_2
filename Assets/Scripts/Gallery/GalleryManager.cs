using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject galleryPolaroid;

    private List<string> dbImageKeys = new();

    // Start is called before the first frame update
    void Start()
    {
        FirebaseManager.Instance.userDatabaseImages.ChildAdded += AddKey;
    }

    //private async void GetAllKeys(DatabaseReference dbRef)
    //{
    //    DataSnapshot imagesSnapshot = await FirebaseManager.Instance.GetDataSnapshot(dbRef);

    //    foreach (DataSnapshot image in imagesSnapshot.Children)
    //    {
    //        string key = image.Key.ToString();
    //        dbImageKeys.Add(key);
    //    }
    //}

    private void AddKey(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        string key = snapshot.Key.ToString();
        dbImageKeys.Add(key);


        Debug.Log(dbImageKeys.Count);
        PrintList();
    }

    private void PrintList()
    {
        foreach (string str in dbImageKeys)
        {
            Debug.Log(str);
        }
    }

    public async void PopulateGallery()
    {
        // Gives us the image data paired with their names
        List<(string, byte[])> images = await FirebaseManager.Instance.DownloadAllImages(dbImageKeys);
    }
}
