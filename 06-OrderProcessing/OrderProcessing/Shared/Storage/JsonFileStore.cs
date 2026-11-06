
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderProcessing.Shared.Storage;

public class JsonFileStore
{
    // Shared JSON settings
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    // One lock for each file

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> FileLocks = new();



    private static readonly Encoding Utf8 = new UTF8Encoding(false);

    // Get the full path
    private static string GetPath(string fileName)
    {
        return DataFiles.GetFilePath(fileName);
    }

    // Get the lock for a specific file
    private static SemaphoreSlim GetLock(string path)
    {
        return FileLocks.GetOrAdd(
            path,
            _ => new SemaphoreSlim(1, 1)
        );
    }

    
    // ASYNC READ
    

    public async Task<List<T>> ReadAllAsync<T>(
        string fileName,
        CancellationToken ct)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        await semaphore.WaitAsync(ct);

        try
        {
            return await ReadInternalAsync<T>(path, ct);
        }
        catch (JsonException ex)
        {
            throw new DataFileCorruptedException(fileName, ex);
        }
        finally
        {
            semaphore.Release();
        }
    }

   
    // INTERNAL ASYNC READ
   

    private async Task<List<T>> ReadInternalAsync<T>(
        string path,
        CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return new List<T>();
        }

        string json = await File.ReadAllTextAsync(path, ct);

        List<T>? data =
            JsonSerializer.Deserialize<List<T>>(json, Options);

        return data ?? new List<T>();
    }

    // =========================
    // ASYNC WRITE
    // =========================

    public async Task WriteAllAsync<T>(
        string fileName,
        IEnumerable<T> data,
        CancellationToken ct)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        await semaphore.WaitAsync(ct);

        try
        {
            await WriteInternalAsync(path, data, ct);
        }
        finally
        {
            semaphore.Release();
        }
    }

    
    // INTERNAL ASYNC WRITE
    

    private async Task WriteInternalAsync<T>(
        string path,
        IEnumerable<T> data,
        CancellationToken ct)
    {
        string? directory = Path.GetDirectoryName(path);

        if (directory != null)
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(data, Options);

        // Write to a temporary file first
        string tempPath = path + "." +
            Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await File.WriteAllTextAsync(
                tempPath,
                json,
                Utf8,
                ct);

            ct.ThrowIfCancellationRequested();

            // Replace the original file
            File.Move(tempPath, path, true);
        }
        finally
        {
            // Remove temporary file if it still exists
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    
    // SAFE ASYNC UPDATE
    

    public async Task<TResult> UpdateAsync<T, TResult>(
        string fileName,
        Func<List<T>, TResult> update,
        CancellationToken ct)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        await semaphore.WaitAsync(ct);

        try
        {
            List<T> data;

            try
            {
                data = await ReadInternalAsync<T>(path, ct);
            }
            catch (JsonException ex)
            {
                throw new DataFileCorruptedException(fileName, ex);
            }

            // Modify the collection
            TResult result = update(data);

            // Save the updated collection
            await WriteInternalAsync(path, data, ct);

            return result;
        }
        finally
        {
            semaphore.Release();
        }
    }

    
    // SYNC READ
    // Used only by sync comparison
    

    public List<T> ReadAll<T>(string fileName)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        semaphore.Wait();

        try
        {
            if (!File.Exists(path))
                return new List<T>();

            string json = File.ReadAllText(path, Utf8);

            return JsonSerializer.Deserialize<List<T>>(
                json, Options) ?? new List<T>();
        }
        catch (JsonException ex)
        {
            throw new DataFileCorruptedException(fileName, ex);
        }
        finally
        {
            semaphore.Release();
        }
    }

    // SYNC WRITE
    // Used only by sync comparison
 

    public void WriteAll<T>(
        string fileName,
        IEnumerable<T> data)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        semaphore.Wait();

        try
        {
            WriteInternal(path, data);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private void WriteInternal<T>(
        string path,
        IEnumerable<T> data)
    {
        string? directory = Path.GetDirectoryName(path);

        if (directory != null)
            Directory.CreateDirectory(directory);

        string json = JsonSerializer.Serialize(data, Options);

        string tempPath = path + "." +
            Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            File.WriteAllText(tempPath, json, Utf8);

            File.Move(tempPath, path, true);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    
    // SYNC UPDATE
   

    public TResult Update<T, TResult>(
        string fileName,
        Func<List<T>, TResult> update)
    {
        string path = GetPath(fileName);

        SemaphoreSlim semaphore = GetLock(path);

        semaphore.Wait();

        try
        {
            List<T> data;

            try
            {
                if (!File.Exists(path))
                    data = new List<T>();
                else
                {
                    string json = File.ReadAllText(path, Utf8);

                    data = JsonSerializer.Deserialize<List<T>>(
                        json, Options) ?? new List<T>();
                }
            }
            catch (JsonException ex)
            {
                throw new DataFileCorruptedException(fileName, ex);
            }

            TResult result = update(data);

            WriteInternal(path, data);

            return result;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
