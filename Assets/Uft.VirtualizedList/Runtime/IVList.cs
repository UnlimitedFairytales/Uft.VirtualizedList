#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList
{
    /// <summary>
    /// 上から順に配置される想定の仮想List
    /// </summary>
    public interface IVList<TListItem, TData>
        where TListItem : MonoBehaviour, IVListItem<TData>
    {
        RectTransform RectTransform { get; }
        VerticalLayoutGroup? VerticalLayoutGroup { get; }
        int InitialPoolLength { get; }
        TListItem? ItemPrototype { get; }
        List<TData> DataList { get; }
        Queue<TListItem> Pool { get; }
        List<TListItem> ActiveList { get; }
        int ActiveStartDataIndex { get; set; }

        void AwakeLogic()
        {
            if (this.VerticalLayoutGroup != null)
            {
                this.VerticalLayoutGroup.enabled = false; // NOTE: ほしいのはpaddingとspacingだけで、これは無効化する必要がある
            }

            if (this.ItemPrototype == null) return;

            for (int i = 0; i < this.InitialPoolLength; i++)
            {
                var item = Object.Instantiate(this.ItemPrototype, this.RectTransform, false);
                item.gameObject.SetActive(false);
                this.Pool.Enqueue(item);
            }
        }

        bool IsActive(int dataIndex) => this.ActiveStartDataIndex <= dataIndex && dataIndex < this.ActiveStartDataIndex + this.ActiveList.Count;

        TListItem ActivateItem(int dataIndex)
        {
            TListItem item = (this.Pool.Count > 0) ?
                this.Pool.Dequeue() :
                Object.Instantiate(this.ItemPrototype!, this.RectTransform);
            item.gameObject.SetActive(true);

            item.gameObject.SetActive(true);
            return item;
        }

        void PositionActiveItem(int dataIndex, TListItem activeItem)
        {
            var vlg = this.VerticalLayoutGroup!;

            // 上から順番に足していく (実際には上先頭 -> 下末尾なので、適用する際は正負反転)
            float y = vlg.padding.top;
            for (int i = 0; i < dataIndex; i++)
            {
                y += this.ItemPrototype!.CalcHeight(this.DataList[i]) + vlg.spacing;
            }

            // anchor・Pivotを左上に念の為にする
            var rt = activeItem.RectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);

            // 配置
            rt.anchoredPosition = new Vector2(0, -y);
        }

        void RefreshVirtualContentHeight()
        {
            if (this.VerticalLayoutGroup == null) return;
            if (this.ItemPrototype == null) return;

            var rt = this.RectTransform;
            float totalHeight = 0;

            // 1. VerticalLayoutGroup.padding
            totalHeight += this.VerticalLayoutGroup.padding.top + this.VerticalLayoutGroup.padding.bottom;

            // 2. All item
            int itemCount = this.DataList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                totalHeight += this.ItemPrototype.CalcHeight(this.DataList[i]);
            }

            // 3. VerticalLayoutGroup.spacing
            if (itemCount > 1)
            {
                totalHeight += this.VerticalLayoutGroup.spacing * (itemCount - 1);
            }

            Vector2 size = rt.sizeDelta;
            size.y = totalHeight;
            rt.sizeDelta = size;
        }

        bool ActiveRangeIsChanged(int actualActiveFirstDataIndex, int actualActiveLastDataIndex)
        {
            return
                this.ActiveStartDataIndex != actualActiveFirstDataIndex ||
                this.ActiveStartDataIndex + this.ActiveList.Count - 1 != actualActiveLastDataIndex;
        }

        void RefreshActiveRange(int actualActiveFirstDataIndex, int actualActiveLastDataIndex)
        {
            // 範囲が不正なら全部非アクティブにして終了
            if (actualActiveLastDataIndex < actualActiveFirstDataIndex)
            {
                for (int i = 0; i < this.ActiveList.Count; i++)
                {
                    var item = this.ActiveList[i];
                    item.gameObject.SetActive(false);
                    this.Pool.Enqueue(item);
                }
                this.ActiveList.Clear();
                this.ActiveStartDataIndex = 0;
                return;
            }

            // いったん全部プールに戻す
            for (int i = 0; i < this.ActiveList.Count; i++)
            {
                var item = this.ActiveList[i];
                item.gameObject.SetActive(false);
                this.Pool.Enqueue(item);
            }
            this.ActiveList.Clear();

            // 必要な範囲を作り直す
            for (int dataIndex = actualActiveFirstDataIndex;
                 dataIndex <= actualActiveLastDataIndex;
                 dataIndex++)
            {
                var item = this.ActivateItem(dataIndex);
                item.SetData(this.DataList[dataIndex]);
                this.PositionActiveItem(dataIndex, item);
                this.ActiveList.Add(item);
            }

            this.ActiveStartDataIndex = actualActiveFirstDataIndex;
        }
    }
}
