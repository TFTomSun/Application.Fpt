namespace Thomas.Apis.Core.DotNet
{
    public interface IToContainer<out T>
    {
        T Instance { get; }
    }
}
