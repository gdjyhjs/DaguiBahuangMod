using System;
using UnityEngine;

namespace GuiBaseUI
{
    public class ItemCell : MonoBehaviour
    {
        public ItemCell(IntPtr ptr) : base(ptr) { }

        public RectTransform rectTransform;
        public virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

        }

        public void SetActive(bool value)
        {
            if (gameObject.activeSelf != value)
            {
                gameObject.SetActive(value);
            }
        }
    }
}