using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Storage;
using System.Threading.Tasks;
using TMPro;
using Firebase.Extensions;
using Unity.VisualScripting;
using System.Threading;

[DefaultExecutionOrder(-2)]
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private FirebaseDatabase _database;
    private DatabaseReference _databaseReference;

    private FirebaseStorage _storage;
    private StorageReference _storageReference;

    private string _userId;

    public DatabaseReference userDatabaseRef;
    public DatabaseReference userDatabaseImages;

    private StorageReference _userStorageRef;


    [SerializeField]
    private TMP_InputField inputField;

    private string _dbImgName = "images";
    private string _strgImagePath = "gs://ar-camera-de5b2.appspot.com";


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        //await UpdateGooglePlayServices();

        inputField.onEndEdit.AddListener(delegate { UploadImage(inputField.text); });

        _database = FirebaseDatabase.DefaultInstance;
        _databaseReference = _database.RootReference;

        _storage = FirebaseStorage.DefaultInstance;
        _storageReference = _storage.RootReference;

        _userId = SystemInfo.deviceUniqueIdentifier;
        userDatabaseRef = _databaseReference.Child(_userId);
        userDatabaseImages = userDatabaseRef.Child(_dbImgName);

        _userStorageRef = _storageReference.Child(_userId);
    }

    private async Task<Task> UpdateGooglePlayServices()
    {
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                var app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        return Task.CompletedTask;
    }

    #region Uploading

    public async void UploadImage(string fileName)
    {
        await UploadImageToStorage(PhotoCapture.Instance.photoDisplayArea.sprite.texture, fileName);
        inputField.text = "";
    }

    public async Task<string> UploadImageToStorage(Texture2D _image, string fileName)
    {
        byte[] imageBytes = _image.EncodeToPNG();
        StorageReference newUpload = _userStorageRef.Child(fileName + ".png");

        var imageMetadata = new MetadataChange();
        imageMetadata.ContentType = "image/png";

        await newUpload.PutBytesAsync(imageBytes, imageMetadata, null, CancellationToken.None)
            .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                }
                else
                {
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished Uploading, md5 hash = " + md5Hash);

                }
            });

        string hash = StoreDownloadUrlInDatabase(newUpload);
        return hash;
    }

    private string StoreDownloadUrlInDatabase(StorageReference reference)
    {
        string url = GetDownloadUrlFromStorage(reference);
        string hash = Utils.md5(url);

        userDatabaseImages.Child(hash).SetValueAsync(url);
        return hash;
    }

    private string GetDownloadUrlFromStorage(StorageReference reference)
    {
        string gsUrl = _strgImagePath + reference.Path;
        Debug.Log($"The storage url is {gsUrl}");
        return gsUrl;
    }

    #endregion


    #region Retrieve Stored Images

    public async Task<List<(string, byte[])>> DownloadAllImages(List<string> keys) 
    {
        List<(string, byte[])> images = new();
        
        foreach (string key in keys) 
        {
            images.Add(await DownloadImage(key));
        }

        Debug.Log(images.Count);
        return images;
    }

    public async Task<(string, byte[])> DownloadImage(string key)
    {
        DatabaseReference dbRef = userDatabaseImages.Child(key);
        string imagePath = await GetDatabaseValue(dbRef);

        StorageReference imageReference = _storage.GetReferenceFromUrl(imagePath);
        string name = imageReference.Name;
        byte[] image = await DownloadImageFromStorage(imageReference);


        return (name, image);
    }

    private async Task<string> GetDatabaseValue(DatabaseReference dbRef)
    {
        DataSnapshot snapshot = await GetDataSnapshot(dbRef);
        string value = snapshot.Value.ToString();

        Debug.Log($"Retrieved value {value} from database key");
        return value;
    }

    public async Task<DataSnapshot> GetDataSnapshot(DatabaseReference dbRef)
    {
        DataSnapshot snapshot = null;

        await dbRef.GetValueAsync().ContinueWithOnMainThread((Task<DataSnapshot> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
            }
            else
            {
                snapshot = task.Result;
                Debug.Log($"Retrieved Data Snapshot");
            }
        });

        return snapshot;
    }

    private async Task<byte[]> DownloadImageFromStorage(StorageReference imageReference) 
    {
        int max_size = 10 * 1024 * 1024; // 10MB
        byte[] imageContents = new byte[2];

        await imageReference.GetBytesAsync(max_size).ContinueWithOnMainThread((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
            }
            else
            {
                imageContents = task.Result;
                Debug.Log("Downloaded 1 image");
            }
        });

        Debug.Log($"The bytes length when received is ${imageContents.Length}");
        return imageContents;
    }

    #endregion

    #region Delete Stored Images

    public async Task<Task> DeleteStoredData(string key)
    {
        DatabaseReference dbRef = userDatabaseImages.Child(key);
        string imagePath = await GetDatabaseValue(dbRef);
        await DeleteDatabaseRef(dbRef);

        StorageReference strgReference = _storage.GetReferenceFromUrl(imagePath);
        await DeleteStorageRef(strgReference);

        return Task.CompletedTask;
    }

    private async Task<Task> DeleteDatabaseRef(DatabaseReference dbRef)
    {
        await dbRef.RemoveValueAsync().ContinueWithOnMainThread((Task task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
            }
            else Debug.Log("File deleted from storage successfully");
        });

        return Task.CompletedTask;
    }

    private async Task<Task> DeleteStorageRef(StorageReference strgRef)
    {
        await strgRef.DeleteAsync().ContinueWithOnMainThread((Task task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
            }
            else Debug.Log("File deleted from storage successfully");
        });

        return Task.CompletedTask;
    }

    #endregion
}
