using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcApp.App_Start
{
    /// <summary>
    /// Implements an Azure Storage (BLOBs) helper to deal with read/write operations on Azure Storage Blobs
    /// <see cref="https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/"/>
    /// </summary>
    public class AzureBlobsStorageHelper : IFileStorageHelper
    {
        #region Properties & Fields

        private CloudStorageAccount mStorageAccount = null;

        private CloudBlobClient mBlobClient = null;

        #endregion

        #region .ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobsStorageHelper"/> class.
        /// </summary>
        public AzureBlobsStorageHelper()
        {
            try
            {
                mStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                mBlobClient = mStorageAccount.CreateCloudBlobClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region ReadToStream

        /// <summary>
        /// Reads to stream, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the sub folder.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<Stream> ReadToStream(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference("folderName");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            Stream fileStream = null;
            await blockBlob.DownloadToStreamAsync(fileStream);

            return fileStream;
        }

        #endregion

        #region ReadToBytes

        /// <summary>
        /// Reads to bytes, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">The containerName.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<byte[]> ReadToBytes(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference("folderName");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            Byte[] fileBytes = null;
            await blockBlob.DownloadToByteArrayAsync(fileBytes, 0);

            return fileBytes;
        }

        #endregion

        #region TempReadToStream

        /// <summary>
        /// Reads to stream.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the sub folder.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Stream TempReadToStream(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference("folderName");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            Stream fileStream = null;
            blockBlob.DownloadToStream(fileStream);

            return fileStream;
        }

        #endregion

        #region TempReadToBytes

        /// <summary>
        /// Reads to bytes.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">The containerName.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public byte[] TempReadToBytes(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference("folderName");

            container.CreateIfNotExists();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            Byte[] fileBytes = null;
            blockBlob.DownloadToByteArray(fileBytes, 0);

            return fileBytes;
        }

        #endregion

        #region WriteFromStream

        /// <summary>
        /// Writes from stream, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">The containerName.</param>
        /// <param name="content">The content.</param>
        public async Task WriteFromStream(string fileName, string containerName, Stream content)
        {
            if (content != null)
            {
                CloudBlobContainer container = mBlobClient.GetContainerReference(containerName);

                container.CreateIfNotExists();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                await blockBlob.UploadFromStreamAsync(content);
            }
        }

        #endregion

        #region WriteFromBytes

        /// <summary>
        /// Writes from bytes, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">The containerName.</param>
        /// <param name="content">The content.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task WriteFromBytes(string fileName, string containerName, byte[] content)
        {
            if (content != null && content.Length > 0)
            {
                CloudBlobContainer container = mBlobClient.GetContainerReference(containerName);

                container.CreateIfNotExists();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                Stream fileStream = new MemoryStream();
                await fileStream.WriteAsync(content, 0, content.Length);
                fileStream.Seek(0, SeekOrigin.Begin);

                await blockBlob.UploadFromStreamAsync(fileStream);
            }
        }

        #endregion

        #region FileExists

        /// <summary>
        /// Check if file exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="subFolderName">Name of the sub folder.</param>
        /// <returns></returns>
        public bool FileExists(string fileName, string containerName)
        {
            return mBlobClient.GetContainerReference(containerName)
                  .GetBlockBlobReference(fileName)
                  .Exists();
        }

        /// <summary>
        /// Files the exists, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">The containerName.</param>
        /// <returns></returns>
        public async Task<bool> FileExistsAsync(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Delete the blob.
            return await blockBlob.ExistsAsync();
        }

        #endregion

        #region RemoveFile

        /// <summary>
        /// Removes the file, asynchronously.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the container.</param>
        public async Task RemoveFile(string fileName, string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Delete the blob.
            await blockBlob.DeleteIfExistsAsync();
        }

        #endregion

        #region GetFiles

        /// <summary>
        /// Gets the files, asynchronously.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        public async Task<IList<Uri>> GetFiles(string containerName)
        {
            CloudBlobContainer container = mBlobClient.GetContainerReference(containerName);

            int i = 0;
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            IList<Uri> files = new List<Uri>();

            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, continuationToken, null, null);

                foreach (var blobItem in resultSegment.Results)
                {
                    files.Add(blobItem.StorageUri.PrimaryUri);
                }

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return files;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mBlobClient = null;
                mStorageAccount = null;
            }
        }

        ~AzureBlobsStorageHelper()
        {
            Dispose(false);
        }

        #endregion
    }
}