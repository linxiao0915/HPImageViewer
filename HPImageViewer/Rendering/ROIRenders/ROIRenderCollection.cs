using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HPImageViewer.Rendering.ROIRenders
{
    public class ROIRenderCollection : IList<ROIRender>
    {
        public event EventHandler RoisChanged;

        private ROIRenderCollection()
        {

        }
        public static ROIRenderCollection CreateByROIDescs(List<ROIDesc> roiDescs, ICoordTransform coordTransform)
        {
            var rOIRenderCollection = new ROIRenderCollection();
            rOIRenderCollection.RoiDescs = roiDescs;

            roiDescs.ForEach(n =>
            {
                var render = RenderFactory.CreateROIRender(n);
                render.RenderTransform = coordTransform;
                render.PropertyChanged += rOIRenderCollection.Item_PropertyChanged;
                rOIRenderCollection.ROIRenders.Add(render);
            });



            return rOIRenderCollection;

        }
        public List<ROIDesc> RoiDescs { get; private set; } = new List<ROIDesc>();
        public List<ROIRender> ROIRenders { get; } = new List<ROIRender>();
        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ROIRender> GetEnumerator()
        {
            return ROIRenders.GetEnumerator();

        }


        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(ROIRender item)
        {
            Insert(0, item);
            //item.PropertyChanged += Item_PropertyChanged;
            //ROIRenders.Add(item);
            //RoiDescs.Add(item.ROIDesc);
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RoisChanged?.Invoke(this, null);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Clear()
        {
            ROIRenders.ToList().ForEach(n => Remove(n));
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(ROIRender item)
        {
            return ROIRenders.Contains(item);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(ROIRender[] array, int arrayIndex)
        {
            ROIRenders.CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(ROIRender item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            RoiDescs.Remove(item.ROIDesc);
            var ok = ROIRenders.Remove(item);
            if (ok)
            {
                RoisChanged?.Invoke(this, null);
            }
            return ok;

        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count => ROIRenders.Count;

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>
        /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, <see langword="false" />.</returns>
        public bool IsReadOnly => false;

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(ROIRender item)
        {
            return ROIRenders.IndexOf(item);

        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void Insert(int index, ROIRender item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanged += Item_PropertyChanged;
            RoiDescs.Insert(index, item.ROIDesc);
            ROIRenders.Insert(index, item);
            RoisChanged?.Invoke(this, null);
        }

        /// <summary>Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void RemoveAt(int index)
        {
            var item = ROIRenders[index];
            Remove(item);
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        /// <returns>The element at the specified index.</returns>
        public ROIRender this[int index]
        {
            get => ROIRenders[index];
            set => ROIRenders[index] = value;
        }

        public List<ROIRender> GetSelectedROIs()
        {
            return ROIRenders.Where(n => n.IsSelected).ToList();
        }

        public void Render(RenderContext renderContext)
        {
            foreach (var roiRender in this)
            {
                roiRender.Render(renderContext);
            }

            AddingRoiRender?.Render(renderContext);
        }

        /// <summary>
        /// 正在添加的ROI
        /// </summary>
        public ROIRender AddingRoiRender { get; set; }
    }
}
