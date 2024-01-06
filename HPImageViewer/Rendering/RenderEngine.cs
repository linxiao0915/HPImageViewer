using System;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace HPImageViewer.Rendering
{
    internal class RenderEngine
    {
        private readonly ActionBlock<RenderDataItem> _renderDataSessionTransformBlock;

        private long _currentRenderSessionTimestampTick = 0;
        public event EventHandler<ImageRender> RenderRequested;
        private readonly object _syncLock = new object();
        public RenderEngine()
        {
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions();
            executionDataflowBlockOptions.EnsureOrdered = true;
            executionDataflowBlockOptions.MaxDegreeOfParallelism = -1;
            executionDataflowBlockOptions.BoundedCapacity = -1;
            _renderDataSessionTransformBlock = new ActionBlock<RenderDataItem>(renderDataItem =>
            {
                var imageRender = renderDataItem.RenderSession.RenderData();

                lock (_syncLock)
                {
                    if (renderDataItem.TimestampTick < _currentRenderSessionTimestampTick)
                        return;
                    _currentRenderSessionTimestampTick = renderDataItem.TimestampTick;

                }
                RenderRequested?.Invoke(this, imageRender);

            }, executionDataflowBlockOptions);



        }
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public void PostRenderSession(RenderSession renderSession)
        {
            _renderDataSessionTransformBlock.Post(new RenderDataItem(renderSession, _stopwatch.ElapsedTicks));
        }


        class RenderDataItem
        {
            public RenderSession RenderSession { get; }
            public long TimestampTick { get; }
            public RenderDataItem(RenderSession renderSession, long timestampTick)
            {
                RenderSession = renderSession;
                TimestampTick = timestampTick;
            }



        }

    }
}
