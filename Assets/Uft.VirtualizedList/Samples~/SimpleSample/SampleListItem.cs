#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.VirtualizedList.Sample
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LayoutElement))]
    public class SampleListItem : MonoBehaviour, IVListItem<(string name, string text)>
    {
        // レイアウト制御用
        public RectTransform RectTransform => (RectTransform)this.transform;
        [SerializeField] LayoutElement? _layoutElement; public LayoutElement? LayoutElement => this._layoutElement;
        [SerializeField] RectTransform? _childVariableSizeSource; public RectTransform? ChildVariableSizeSource => this._childVariableSizeSource;
        [SerializeField] float _fixedOtherWidth; public float FixedOtherWidth => this._fixedOtherWidth;
        [SerializeField] float _fixedOtherHeight; public float FixedOtherHeight => this._fixedOtherHeight;
        [SerializeField] float _fallbackChildVariableWidth; public float FallbackChildVariableWidth => this._fallbackChildVariableWidth;
        [SerializeField] float _fallbackChildVariableHeight; public float FallbackChildVariableHeight => this._fallbackChildVariableHeight;

        // 仮想計算用
        [SerializeField] float _virtualLineHeight = 30f;
        [SerializeField] float _virtualPadding = 20f;

        // 実際のParameter
        [SerializeField] TMP_Text? _txtName;
        [SerializeField] TMP_Text? _txtText;

        // Unity events

        void Reset()
        {
            if (this._layoutElement == null) this._layoutElement = this.GetComponent<LayoutElement>();
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) this.RefreshLayoutElement();
#endif
        }

        // Methods

        public void SetData((string name, string text) data)
        {
            if (this._txtName != null) this._txtName.text = data.name;
            if (this._txtText != null)
            {
                this._txtText.text = data.text;
                this.RefreshLayoutElement();
            }
        }

        public float CalcHeight((string name, string text) data)
        {
            string text = data.text;
            int lineCount = CountLines(text);
            float itemHeight = this._virtualPadding + this._virtualLineHeight * lineCount;
            return itemHeight;

            // 折り返しない前提で行数を計算
            static int CountLines(string text)
            {
                if (string.IsNullOrEmpty(text)) return 1;

                int count = 1;
                var prevIsTmpNewLineFirstChar = false;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n') count++;
                    if (prevIsTmpNewLineFirstChar && text[i] == 'n') count++;
                    prevIsTmpNewLineFirstChar = (text[i] == '\\');
                }
                return count;
            }
        }

        void RefreshLayoutElement() => ((IVListItem<(string name, string text)>)this).RefreshLayoutElement();
    }
}
