namespace CapW.Storage;

public interface IStorageService
{
    public void StoreSimple(string key, string value);
    public void StoreSimple(string key, bool value);
    public void StoreSimple<TNumber>(string key, TNumber value) where TNumber : INumber<TNumber>;
    public string? RetrieveString(string key);
    public bool RetrieveBool(string key);
    public INumber<TNumber> RetrieveNumber<TNumber>(string key) where TNumber : INumber<TNumber>;
    public void DeleteSimple(string key);
    public ValueTask StoreToJsonFileAsync<TValue>(string filename, TValue value);
    public ValueTask<TValue?> RetrieveFromJsonFileAsync<TValue>(string filename);
    public ValueTask DeleteJsonFileAsync(string filename);
}

public sealed class StorageService : IStorageService
{
    public StorageService()
    {
        _storageContainer = ApplicationData.Current.LocalSettings;
        _storageFolder = ApplicationData.Current.LocalCacheFolder;
    }

    private readonly ApplicationDataContainer _storageContainer;
    private readonly StorageFolder _storageFolder;

    public void StoreSimple(string key, string value) => _storageContainer.Values[key] = value;

    public void StoreSimple(string key, bool value) => _storageContainer.Values[key] = value;

    public void StoreSimple<TNumber>(string key, TNumber value) where TNumber : INumber<TNumber> => _storageContainer.Values[key] = value;

    public string? RetrieveString(string key)
    {
        if (_storageContainer.Values.TryGetValue(key, out var value))
            return value as string;

        return default;
    }

    public bool RetrieveBool(string key)
    {
        if (_storageContainer.Values.TryGetValue(key, out var value))
        {
            if (value is bool boolean)
                return boolean;

            if (value is short number)
                return number == 1;

            if (value is string str)
            {
                if ("1".Equals(str, StringComparison.Ordinal))
                    return true;

                if ("true".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return true;

                if ("yes".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return default;
    }

    public INumber<TNumber> RetrieveNumber<TNumber>(string key) where TNumber : INumber<TNumber>
    {
        if (_storageContainer.Values.TryGetValue(key, out var value))
        {
            if (value is TNumber number)
                return number;

            if (value is string str)
            {
                if (TNumber.TryParse(s: str, provider: CultureInfo.InvariantCulture, out var parsed))
                    return parsed;
            }

            if (value is sbyte sbyteNumber)
                return TNumber.CreateChecked(sbyteNumber);

            if (value is short shortNumber)
                return TNumber.CreateChecked(shortNumber);

            if (value is int intNumber)
                return TNumber.CreateChecked(intNumber);

            if (value is long longNumber)
                return TNumber.CreateChecked(longNumber);

            if (value is byte byteNumber)
                return TNumber.CreateChecked(byteNumber);

            if (value is ushort ushortNumber)
                return TNumber.CreateChecked(ushortNumber);

            if (value is uint uintNumber)
                return TNumber.CreateChecked(uintNumber);

            if (value is ulong ulongNumber)
                return TNumber.CreateChecked(ulongNumber);

            if (value is float floatNumber)
                return TNumber.CreateChecked(floatNumber);

            if (value is double doubleNumber)
                return TNumber.CreateChecked(doubleNumber);

            if (value is decimal decimalNumber)
                return TNumber.CreateChecked(decimalNumber);
        }

        return TNumber.Zero;
    }

    public void DeleteSimple(string key)
    {
        if (_storageContainer.Values.ContainsKey(key))
            _storageContainer.Values.Remove(key);
    }

    public async ValueTask StoreToJsonFileAsync<TValue>(string filename, TValue value)
    {
        var fileName = Path.ChangeExtension(filename, ".json");
        var fullFilePath = Path.Join(_storageFolder.Path, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
        using var stream = File.Open(fullFilePath, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, value);
        await stream.FlushAsync();
    }

    public async ValueTask<TValue?> RetrieveFromJsonFileAsync<TValue>(string filename)
    {
        var fileName = Path.ChangeExtension(filename, ".json");
        var fullFilePath = Path.Join(_storageFolder.Path, fileName);

        if (File.Exists(fullFilePath) is false)
            return default;

        using var stream = File.Open(fullFilePath, FileMode.Open);
        var value = await JsonSerializer.DeserializeAsync<TValue>(stream);
        return value;
    }

    public ValueTask DeleteJsonFileAsync(string filename)
    {
        var fileName = Path.ChangeExtension(filename, ".json");
        var fullFilePath = Path.Join(_storageFolder.Path, fileName);

        if (File.Exists(fullFilePath))
            File.Delete(fullFilePath);

        return ValueTask.CompletedTask;
    }
}
