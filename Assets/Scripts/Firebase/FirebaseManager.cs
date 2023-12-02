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
            .ContinueWith((Task<StorageMetadata> task) =>
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
        string key = Utils.md5(url);

        _userDatabaseRef.Child(_dbImgName).Child(key).SetValueAsync(url);
    }

    private string GetDownloadUrlFromStorage(StorageReference reference)
    {
        string gsUrl = _strgImagePath + reference.Path;
        Debug.Log($"The storage url is {gsUrl}");
        return gsUrl;
    }

    #endregion


    #region Retrieve Stored Images

    private void DownloadAllImages()
    {
       // var root = _userStorageRef.listAll();
    }

    /*
    private async Task<List<FileMetadata>> GetMetadataList(string path)
    {
        const string baseApiUrl = "https://firebasestorage.googleapis.com/v0/b/";
        const string projectId = "ar-camera-de5b2";

        string userImageUrl = $"{baseApiUrl}{projectId}/o?prefix={path}";

        using (HttpClient client = new HttpClient())
        using (HttpResponseMessage response = await client.GetAsync(url))
        using (HttpContent content = response.Content)
        {
            // Read the response body
            string responseText = await content.ReadAsStringAsync();

            // Deserialize the JSON response
            ListFilesResponse responseData = JsonConvert.DeserializeObject<ListFilesResponse>(responseText);

            // Return the list of files
            return responseData.Files;
        }

    }
    */
    private byte[] DownloadImage(StorageReference imageReference) 
    {
        int max_size = 1024 * 1024; // 1MB
        byte[] imageContents = new byte[64];

        imageReference.GetBytesAsync(max_size).ContinueWith((Task<byte[]> task) =>
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

    class FileMetadata
    {
        public string Name { get; set; }
        public string DownloadUrl { get; set; }
    }

    class ListFilesResponse
    {
        public List<FileMetadata> Files { get; set; }
    }
}
