using Autofac;

namespace XamaCore
{
    /// <summary>
    /// build the autofac container
    /// </summary> 
    class AutofacContainerInit
    {
        private ContainerBuilder _builder;
        public AutofacContainerInit()
        {
            _builder = new ContainerBuilder();
        }

        /// <summary>
        /// build autofac container and return it
        /// </summary>
        /// <returns><see cref="IContainer">container build and read to use</returns>
        public IContainer Build()
        {
            return _builder.Build();
        }
    }
}
