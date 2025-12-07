#nullable enable

using UnityEngine;

namespace Uft.VirtualizedList.Sample
{
    public class SampleDirector : MonoBehaviour
    {
        [SerializeField] SampleList?  _sampleList;

        void Start()
        {
            // NOTE: Awake間の呼び出し順序は非決定的なので、Startで投入
            this._sampleList!.AddItemData(("Alice", "これは本文です"));
            this._sampleList!.AddItemData(("Bob", "これは本文です\n2行です"));
            this._sampleList!.AddItemData(("Carol", "これは本文です"));
            this._sampleList!.AddItemData(("Dave", "これは本文です。これは本文です"));
            this._sampleList!.AddItemData(("Eve", "盗み聞き中。"));
            this._sampleList!.AddItemData(("Frank", "6番目。"));
            this._sampleList!.AddItemData(("Grace", "7番目。"));
            this._sampleList!.AddItemData(("Heidi", "8番目。"));
            this._sampleList!.AddItemData(("Ivan", "9番目。"));
            for (int i = 0; i < 100; i++)
            {
                var text =
                    i % 3 == 0 ? $"{(10 + i)}番目。":
                    i % 3 == 1 ? $"{(10 + i)}番目。\n2行必要かもしれないからチェック" :
                    $"{(10 + i)}番目。\n3行必要かもしれないからチェック\n3行目はここ";
                this._sampleList!.AddItemData(("John", text));
            }
            this._sampleList!.AddItemData(("Zoe", "私で最後。"));
        }
    }
}
