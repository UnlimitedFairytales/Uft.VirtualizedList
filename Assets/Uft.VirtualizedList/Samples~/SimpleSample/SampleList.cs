#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList.Sample
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class SampleList : MonoBehaviour, IVList<SampleListItem, (string name, string text)>
    {
        public RectTransform RectTransform => (RectTransform)this.transform;
        [SerializeField] VerticalLayoutGroup? _verticalLayoutGroup; public VerticalLayoutGroup? VerticalLayoutGroup => this._verticalLayoutGroup;
        [SerializeField] int _initialPoolLength = 20; public int InitialPoolLength => this._initialPoolLength;
        [SerializeField] SampleListItem? _itemPrototype; public SampleListItem? ItemPrototype => this._itemPrototype;

        readonly List<(string name, string text)> _dataList = new(); public List<(string name, string text)> DataList => this._dataList;
        readonly Queue<SampleListItem> _pool = new(); public Queue<SampleListItem> Pool => this._pool;
        readonly List<SampleListItem> _activeList = new(); public List<SampleListItem> ActiveList => this._activeList;
        int _activeStartDataIndex = 0;
        public int ActiveStartDataIndex
        {
            get { return this._activeStartDataIndex; }
            set { this._activeStartDataIndex = value; }
        }

        // Unity events

        void Reset()
        {
            if (this._verticalLayoutGroup == null) this._verticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();
        }

        void Awake() => this.AwakeLogic();

        // Methods

        public void AddItemData((string name, string text) data)
        {
            this.DataList.Add(data);
            this.RefreshVirtualContentHeight();
        }

        void RefreshVirtualContentHeight() => ((IVList<SampleListItem, (string name, string text)>)this).RefreshVirtualContentHeight();

        void AwakeLogic() => ((IVList<SampleListItem, (string name, string text)>)this).AwakeLogic();
    }
}
