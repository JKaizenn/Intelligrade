using System.Threading.Tasks;

namespace IntelliGrade.App.Interfaces;

/// <summary>
/// Interface for local storage operations.
/// Provides abstraction for persistent storage, enabling different storage backends.
/// </summary>
public interface ILocalStorageService
{
    /// <summary>
    /// Saves a value to local storage.
    /// </summary>
    /// <typeparam name="T">Type of value to store</typeparam>
    /// <param name="key">Storage key</param>
    /// <param name="value">Value to store</param>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Retrieves a value from local storage.
    /// </summary>
    /// <typeparam name="T">Type of value to retrieve</typeparam>
    /// <param name="key">Storage key</param>
    /// <param name="defaultValue">Default value if key not found</param>
    /// <returns>Stored value or default</returns>
    Task<T?> GetAsync<T>(string key, T? defaultValue = default);

    /// <summary>
    /// Removes a value from local storage.
    /// </summary>
    /// <param name="key">Storage key to remove</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Clears all local storage.
    /// </summary>
    Task ClearAsync();
}
