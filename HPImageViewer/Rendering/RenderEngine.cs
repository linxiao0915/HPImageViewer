using System;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace HPImageViewer.Rendering
{
    internal class RenderEngine
    {
        private readonly ActionBlock<RenderDataItem> _renderDataSessionTransformBlock;

        private long _currentRenderSessionTimestampTick = 0;
        public event EventHandler<RenderSet> RenderRequested;
        private readonly object _syncLock = new object();
        private DateTime _lastTick = DateTime.MinValue;
        public RenderEngine()
        {
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions();
            executionDataflowBlockOptions.EnsureOrdered = true;
            executionDataflowBlockOptions.MaxDegreeOfParallelism = 1;
            executionDataflowBlockOptions.BoundedCapacity = -1;
            _renderDataSessionTransformBlock = new ActionBlock<RenderDataItem>(renderDataItem =>
            {

                var needRender = false;
                lock (_syncLock)
                {
                    if (renderDataItem.TimestampTick < _currentRenderSessionTimestampTick)
                        return;
                    _currentRenderSessionTimestampTick = renderDataItem.TimestampTick;
                    needRender = (DateTime.Now - _lastTick).TotalMilliseconds > 30;
                    if (needRender) _lastTick = DateTime.Now;

                }
                if (needRender)
                {
                    var imageRender = renderDataItem.RenderSession.RenderData();
                    RenderRequested?.Invoke(this, new RenderSet(renderDataItem.RenderSession.RenderContext) { ImageRender = imageRender });
                }
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
