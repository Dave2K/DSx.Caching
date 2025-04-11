namespace DSx.Caching.SharedKernel.Validation
{
    public interface ICacheKeyValidator
    {
        void Validate(string key);
        string NormalizeKey(string key);
    }
}