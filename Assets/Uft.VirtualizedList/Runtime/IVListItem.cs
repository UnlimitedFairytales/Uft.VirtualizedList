#nullable enable

using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList
{
    /// <summary>
    /// ChildVariableSizeSource + FixedOtherWidth or FixedOtherHeight を サイズ として LayoutElement で 上位に伝えられるようにしてあるInterface<br/>
    /// HACK: サイズ算出はCalcHeight・CalcWidthで一本化するべきだが、そうなっていない
    /// </summary>
    public interface IVListItem<TData>
    {
        RectTransform RectTransform { get; }

        LayoutElement? LayoutElement { get; }
        RectTransform? ChildVariableSizeSource { get; }
        // float Width => this.LayoutElement != null ? this.LayoutElement.preferredWidth : this.FixedOtherWidth + this.FallbackChildVariableWidth;
        // float Height => this.LayoutElement != null ? this.LayoutElement.preferredHeight : this.FixedOtherHeight + this.FallbackChildVariableHeight;
        float FixedOtherWidth { get; }
        float FixedOtherHeight { get; }
        /// <summary>e.g. 固定幅テキスト200px + 可変幅テキストなら、1行の高さと同じにすると良さげ</summary>
        float FallbackChildVariableWidth { get; }
        /// <summary>e.g. 可変行テキストで1行辺り30px + 枠画像余白上下10pxなら、30+10+10=50にしておくと良さげ</summary>
        float FallbackChildVariableHeight { get; }

        void SetData(TData data);

        float CalcHeight(TData data);

        void RefreshLayoutElement()
        {
            if (this.ChildVariableSizeSource == null || this.LayoutElement == null) return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.ChildVariableSizeSource);
#if UNITY_EDITOR
            if (!Application.isPlaying) Canvas.ForceUpdateCanvases();
#endif
            float w = this.ChildVariableSizeSource.rect.width + this.FixedOtherWidth;
            if (!Mathf.Approximately(this.LayoutElement.preferredWidth, w))
            {
                this.LayoutElement.preferredWidth = w;
            }
            float h = this.ChildVariableSizeSource.rect.height + this.FixedOtherHeight;
            if (!Mathf.Approximately(this.LayoutElement.preferredHeight, h))
            {
                this.LayoutElement.preferredHeight = h;
            }
        }
    }
}
