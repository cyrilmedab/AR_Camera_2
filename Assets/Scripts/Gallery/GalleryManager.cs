using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    [SerializeField]
    private Transform galleryScrollContent;

    [SerializeField]
    private GameObject galleryPolaroidPrefab;

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

        PopulateGallery(key);

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

    public async void PopulateGallery(string key) 
    {
        (string, byte[]) photo = await FirebaseManager.Instance.DownloadImage(key);

        GameObject newPolaroid = Instantiate(galleryPolaroidPrefab, galleryScrollContent);

        if (newPolaroid.TryGetComponent<GalleryPolaroid>(out GalleryPolaroid galleryPolaroid))
        {
            galleryPolaroid.SetNameAndImage(photo);
        }
    }


}
