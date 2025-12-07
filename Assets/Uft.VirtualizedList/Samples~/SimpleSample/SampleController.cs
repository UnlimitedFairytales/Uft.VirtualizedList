#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList.Sample
{
    [RequireComponent(typeof(ScrollRect))]
    public class SampleController : MonoBehaviour, IVController<SampleList, SampleListItem, (string name, string text)>
    {
        [SerializeField] ScrollRect? _scrollRect; public ScrollRect? ScrollRect => this._scrollRect;
        [SerializeField] SampleList? _list; public SampleList? Content => this._list;
        [SerializeField] float _margin = 50f; public float Margin => this._margin; 

        public Transform Transform => this.transform;

        void Reset()
        {
            if (this._scrollRect == null) this._scrollRect = this.GetComponent<ScrollRect>();
            if (this._list == null) this._list = this.GetComponentInChildren<SampleList>();
        }

        void Update()
        {
            if (this._scrollRect == null || this._list == null) return;
            this.Tick();
        }

        public int GetIndexAtPosition(float posY)
        {
            if (this._list == null || this._list.ItemPrototype == null) return 0;

            var layoutGroup = this._list.VerticalLayoutGroup;
            if (layoutGroup == null) return 0;

            float y = 0;
            y += layoutGroup.padding.top;
            for (int i = 0; i < this._list.DataList.Count; i++)
            {
                var itemH = this._list.ItemPrototype.CalcHeight(this._list.DataList[i]);
                if (posY < y + itemH) return i;
                y += itemH + layoutGroup.spacing;
            }
            return this._list.DataList.Count - 1;
        }

        void Tick() => ((IVController<SampleList, SampleListItem, (string name, string text)>)this).Tick();
    }
}
