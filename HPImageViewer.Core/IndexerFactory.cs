using System;
using System.Collections.Generic;

namespace HPImageViewer.Core
{
    public interface IPixelIndexerFactory
    {
        public PixelDataIndexer CreatePixelDataIndexer(object image);

    }

    public sealed class AggregationIndexerFactory
    {
        private AggregationIndexerFactory()
        { }

        public static AggregationIndexerFactory Instance { get; } = new();

        private readonly Dictionary<Type, IPixelIndexerFactory> _indexerFactories = new();
        public AggregationIndexerFactory Register<T, U>() where U : IPixelIndexerFactory
        {
            _indexerFactories.Add(typeof(T), Activator.CreateInstance<U>());
            return this;
        }
        public PixelDataIndexer CreatePixelDataIndexer(object image)
        {
            return _indexerFactories[image.GetType()].CreatePixelDataIndexer(image);
        }
    }
}
