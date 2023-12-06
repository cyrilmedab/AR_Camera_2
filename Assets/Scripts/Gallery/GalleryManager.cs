using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }
    public bool changingName = false;

    [SerializeField]
    private Transform galleryScrollContent;

    [SerializeField]
    private GameObject galleryPolaroidPrefab;

    public GameObject fullScreenPopup;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    { 
        FirebaseManager.Instance.userDatabaseImages.ChildAdded += AddKey;
    }

    public void AddKey(object sender, ChildChangedEventArgs args)
    {
        // We don't want to create a new polaroid if we're just changing the name
        if (changingName) return;

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        string key = snapshot.Key.ToString();

        PopulateGallery(key);
    }

    public async void PopulateGallery(string key) 
    {
        (string, byte[]) photo = await FirebaseManager.Instance.DownloadImage(key);

        GameObject newPolaroid = Instantiate(galleryPolaroidPrefab, galleryScrollContent);

        if (newPolaroid.TryGetComponent<GalleryPolaroid>(out GalleryPolaroid galleryPolaroid))
        {
            galleryPolaroid.SetNameAndImage(photo);
        }

        galleryPolaroid.dbHash = key;
    }


}
