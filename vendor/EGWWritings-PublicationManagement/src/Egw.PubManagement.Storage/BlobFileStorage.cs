using System.Data.Common;
using System.Runtime.CompilerServices;

using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Egw.PubManagement.Storage;

/// <inheritdoc cref="IStorage" />
public class BlobFileStorage : IStorage, IDisposable
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly Uri _baseUri;
    private readonly string? _prefix;


    /// <summary> Default constructor </summary>
    public BlobFileStorage(string connectionString)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        var config = new AmazonS3Config
        {
            ServiceURL = builder.GetValueOrDefault("serviceurl", null),
            ForcePathStyle = true,
            AuthenticationRegion = builder.GetValueOrDefault("region", "us-east-1"),
            UseArnRegion = true
        };

        _bucket = builder.GetRequiredValue("bucket");
        _prefix = builder.GetValueOrDefault("prefix", null);
        _baseUri = new Uri(builder.GetValueOrDefault("externalurl", config.ServiceURL)!);
        if (_prefix is not null)
        {
            _baseUri = new Uri(_baseUri, _prefix + '/');
        }

        var credentials = new BasicAWSCredentials(
            builder.GetRequiredValue("username"), builder.GetRequiredValue("password")
        );
        _s3 = new AmazonS3Client(credentials, config);
    }

    /// <inheritdoc />
    public async Task<Stream?> Read(string path, CancellationToken cancellationToken)
    {
        try
        {
            // Console.WriteLine($"{path} => {MakeAbsolutePath(path)}");
            GetObjectResponse? blob = await _s3.GetObjectAsync(_bucket, MakeAbsolutePath(path), cancellationToken);
            return blob?.ResponseStream;
        }
        catch (AmazonS3Exception)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task Write(string path, Stream stream, CancellationToken cancellationToken)
    {
        await _s3.PutObjectAsync(
            new PutObjectRequest { BucketName = _bucket, Key = MakeAbsolutePath(path), InputStream = stream },
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task Delete(string path, CancellationToken cancellationToken)
    {
        await _s3.DeleteObjectAsync(_bucket, MakeAbsolutePath(path), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> Exists(string path, CancellationToken cancellationToken)
    {
        GetObjectMetadataResponse? metadata =
            await _s3.GetObjectMetadataAsync(_bucket, MakeAbsolutePath(path), cancellationToken);
        return metadata is not null;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IStorageBlob> ListObjects(string path,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var listRequest =
            new ListObjectsRequest { BucketName = _bucket, MaxKeys = 1000, Prefix = MakeAbsolutePath(path) + '/' };
        ListObjectsResponse? response;
        do
        {
            response = await _s3.ListObjectsAsync(listRequest, cancellationToken);
            if (response is not null)
            {
                foreach (S3Object? item in response.S3Objects)
                {
                    yield return new S3StorageBlob(item, _prefix);
                }
            }

            if (response?.IsTruncated == true)
            {
                listRequest.Marker = response.NextMarker;
            }
            else
            {
                response = null;
            }
        } while (response is not null);
    }


    /// <inheritdoc />
    public Uri GetUri(string path) => new(_baseUri, path);

    private string MakeAbsolutePath(string path) =>
        string.IsNullOrWhiteSpace(_prefix) ? path : Path.Combine(_prefix, path).Replace('\\', '/');

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _s3.Dispose();
    }
}