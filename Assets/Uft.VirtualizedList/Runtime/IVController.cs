#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList
{
    public interface IVController<TList, TListItem, TData>
        where TList : MonoBehaviour, IVList<TListItem, TData>
        where TListItem : MonoBehaviour, IVListItem<TData>
    {
        ScrollRect? ScrollRect { get; }
        TList? Content { get; }
        RectTransform? ViewportRT => (this.ScrollRect != null) ? this.ScrollRect.viewport : null;
        float Margin { get; }
        float VisibleHeight => this.ViewportRT != null ? this.ViewportRT.rect.height : 0;

        int GetIndexAtPosition(float posY);

        void Tick()
        {
            if (this.ScrollRect == null || this.Content == null) return;

            var (first, last) = this.GetVisibleRange();
            if (this.Content.ActiveRangeIsChanged(first, last))
            {
                this.Content.RefreshActiveRange(first, last);
            }
        }

        (int first, int last) GetVisibleRange()
        {
            if (this.Content == null ||
                this.Content.ItemPrototype == null ||
                this.ScrollRect == null ||
                this.Content.VerticalLayoutGroup == null) return (0, -1);

            float contentY = this.Content.RectTransform.anchoredPosition.y;
            float maxY = contentY + this.ScrollRect.viewport.rect.height;
            float minY = contentY;

            float y = this.Content.VerticalLayoutGroup.padding.top;
            int first = 0, last = -1;
            bool found = false;
            for (int i = 0; i < this.Content.DataList.Count; i++)
            {
                var h = this.Content.ItemPrototype.CalcHeight(this.Content.DataList[i]);
                var top = y;
                var bottom = y + h;

                if (!found && bottom >= (minY - this.Margin))
                {
                    found = true;
                    first = i;
                }
                if (top <= (maxY + this.Margin))
                {
                    last = i;
                }
                else if (found)
                {
                    break;
                }
                y = bottom + this.Content.VerticalLayoutGroup.spacing;
            }

            if (!found) return (0, -1);
            return (first, last);
        }
    }
}
