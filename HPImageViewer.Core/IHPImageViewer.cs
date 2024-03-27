using System;
using HPImageViewer.Core.Persistence;

namespace HPImageViewer.Core
{
    public interface IHPImageViewer : IDocument
    {
        [Obsolete("请使用Image属性")]
        public void SetImage(object Image);
        public object Image { get; set; }

        public void AddROIs(params ROIDesc[] rois);
        public void UpdateROIs(params ROIDesc[] rois);
        //todo:优化成同步方法FitImageToArea；显示图片异步，图片数据同步方法中更新，避免一致性问题导致FitImageToArea()同步方法失效
        bool FitNewImageToArea { get; set; }

        void FitImageToArea();

        public ToolType ActivatedTool { get; set; }

        event EventHandler ShapeDrawCompleted;

    }
}
