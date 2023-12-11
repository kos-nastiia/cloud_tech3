using Azure.Storage.Blobs;
using System;
using System.IO;

public class AzureBlobStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobStorageService(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient ?? throw new ArgumentNullException(nameof(blobContainerClient));
    }

    public string UploadPhoto(string photoFilePath, string contactRowKey)
    {
        if (string.IsNullOrEmpty(photoFilePath))
        {
            return null;
        }

        var blobClient = _blobContainerClient.GetBlobClient($"{contactRowKey}_photo.jpg");
        string photoUrl;

        try
        {
            using (var fileStream = File.OpenRead(photoFilePath))
            {
                blobClient.Upload(fileStream);
            }

            photoUrl = blobClient.Uri.ToString();
        }
        catch (IOException ex)
        {
            // Handle IO exceptions (e.g., file read/write issues)
            Console.WriteLine($"An IO exception occurred: {ex.Message}");
            // Optionally, log the exception or take other appropriate actions
            photoUrl = null; // or set to a default/error value as needed
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"An exception occurred: {ex.Message}");
            // Optionally, log the exception or take other appropriate actions
            photoUrl = null; // or set to a default/error value as needed
        }

        return photoUrl;
    }


    public void DeletePhoto(string contactRowKey)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{contactRowKey}_photo.jpg");

        blobClient.DeleteIfExists();
    }

    public byte[] DownloadPhoto(string contactRowKey)
    {
        var blobClient = _blobContainerClient.GetBlobClient($"{contactRowKey}_photo.jpg");

        // Check if the blob exists before attempting to download
        if (!blobClient.Exists())
        {
            // Handle the case where the blob does not exist
            // You may choose to return a default photo or null, or log a message
            return null;
        }

        using (var memoryStream = new MemoryStream())
        {
            blobClient.DownloadTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

}
