namespace Thomas.Apis.Core.DotNet
{
    internal class ToContainer<T> : IToContainer<T>
    {
        public T Instance { get; }

        public ToContainer(T instance)
        {
            Instance = instance;
        }
    }
}
