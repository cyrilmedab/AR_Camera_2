using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Storage;
using System.Dynamic;
using System.Threading.Tasks;
using TMPro;
using System.Net.Http;
using System;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseDatabase _database;
    private DatabaseReference _databaseReference;

    private FirebaseStorage _storage;
    private StorageReference _storageReference;

    private string _userId;
    private DatabaseReference _userDatabaseRef;
    private StorageReference _userStorageRef;

    [SerializeField]
    private TMP_InputField inputField;

    private string _dbImgName = "images";
    private string _strgImagePath = "gs://ar-camera-de5b2.appspot.com";


    private void Awake()
    {
        inputField.onEndEdit.AddListener(delegate { UploadImage(inputField.text); });
    }

    // Start is called before the first frame update
    private void Start()
    {
        _database = FirebaseDatabase.DefaultInstance;
        _databaseReference = _database.RootReference;

        _storage = FirebaseStorage.DefaultInstance;
        _storageReference = _storage.RootReference;

        _userId = SystemInfo.deviceUniqueIdentifier;
        _userDatabaseRef = _databaseReference.Child(_userId);
        _userStorageRef = _storageReference.Child(_userId);

        CreateNewUser();

    }

    private void CreateNewUser()
    {
        _databaseReference.Child("users").Child(_userId).SetValueAsync("Laptop");
    }

    #region Uploading

    public async void UploadImage(string fileName)
    {
        await UploadImageToStorage(PhotoCapture.Instance.photoDisplayArea.sprite.texture, fileName);
        inputField.text = "";
    }

    private async Task<Task> UploadImageToStorage(Texture2D _image, string fileName)
    {
        byte[] imageBytes = _image.EncodeToPNG();
        StorageReference newUpload = _userStorageRef.Child(fileName + ".png");

        await newUpload.PutBytesAsync(imageBytes)
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

        StoreDownloadUrlInDatabase(newUpload);
        return Task.CompletedTask;
    }

    private void StoreDownloadUrlInDatabase(StorageReference reference)
    {
        string url = GetDownloadUrlFromStorage(reference);
        string hash = Utils.md5(url);

        _userDatabaseRef.Child(_dbImgName).Child(hash).SetValueAsync(url);
    }

    private string GetDownloadUrlFromStorage(StorageReference reference)
    {
        string gsUrl = _strgImagePath + reference.Path;
        Debug.Log($"The storage url is {gsUrl}");
        return gsUrl;
    }

    #endregion


    #region Retrieve Stored Images

    public async void DownloadAllImages(string[] keys) 
    {
        List<byte[]> images = new();
        
        foreach (string key in keys) 
        {
            images.Add(await DownloadImage(key));
        }

        Debug.Log(images.Count);
    }

    public async Task<byte[]> DownloadImage(string key)
    {
        DatabaseReference dbRef = _userDatabaseRef.Child(key);
        string imagePath = await GetDatabaseValue(dbRef);

        StorageReference imageReference = _storage.GetReferenceFromUrl(imagePath);
        byte[] image = DownloadImageFromStorage(imageReference);

        return image;
    }

    private async Task<string> GetDatabaseValue(DatabaseReference dbRef)
    {
        string value = "";

        await dbRef.GetValueAsync().ContinueWithOnMainThread((Task<DataSnapshot> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception);
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                value = snapshot.Value.ToString();
                Debug.Log($"Retrieved value {value} from database key");
            }
        });

        return value;
    }

    private byte[] DownloadImageFromStorage(StorageReference imageReference) 
    {
        int max_size = 1024 * 1024; // 1MB
        byte[] imageContents = new byte[64];

        imageReference.GetBytesAsync(max_size).ContinueWithOnMainThread((Task<byte[]> task) =>
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

        return imageContents;
    }

    #endregion

}
