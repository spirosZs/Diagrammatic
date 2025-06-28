namespace Exercises.Core.Abstractions
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
