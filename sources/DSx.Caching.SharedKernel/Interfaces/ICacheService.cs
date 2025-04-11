// File: sources/DSx.Caching.SharedKernel/Interfaces/ICacheService.cs
using System.Threading.Tasks;

namespace DSx.Caching.SharedKernel.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value);
        Task RemoveAsync(string key);
    }
}