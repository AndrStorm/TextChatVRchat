using UdonSharp;
using UnityEngine;
using UnityEngine.UI;


public class LayoutGroupHelper : UdonSharpBehaviour
{
    [SerializeField] private RectTransform _ItemsListRT;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
    [SerializeField] private Scrollbar _verticalGroupScrollbar;
    [SerializeField] private LayoutElement _EmptyContentLayout;
    [SerializeField] private float _heightOffset = -10f;
    
    
    public void SetUpVerticalLayoutGroup()
    {
        gameObject.SetActive(true);
        //Debug.Log($"SetUpVerticalLayoutGroup 1-{_ItemsListRT} 2-{_ItemsListRT.rect}");
        
        float groupHeight = _ItemsListRT.rect.height;
        float spacing = _verticalLayoutGroup.spacing;
        
        float elementsHeight = 0f;
        int activeElementsCount = 0;
        for(int i = 0; i < _verticalLayoutGroup.transform.childCount; i++)
        {
            var child = _verticalLayoutGroup.transform.GetChild(i);
            if (!child.gameObject.activeSelf) continue;
            if (child == _EmptyContentLayout.transform) continue;
            
            var childRectTransform = child.GetComponent<RectTransform>();
            elementsHeight += childRectTransform.rect.height;
            activeElementsCount++;
        }
        
        float remainingHeight = groupHeight - elementsHeight - spacing * activeElementsCount;
        remainingHeight = Mathf.Max(0f, remainingHeight);
        
        // Debug.Log($"remainingHeight {remainingHeight} groupHeight {groupHeight} " +
        //           $"elementsHeight {elementsHeight} activeElementsCount {activeElementsCount}");
        //_EmptyContent.sizeDelta = new Vector2(_EmptyContent.sizeDelta.x, remainingHeight);
        _EmptyContentLayout.minHeight = remainingHeight + _heightOffset;
        
        _EmptyContentLayout.transform.SetAsFirstSibling();
        if (remainingHeight < 0.1f)
        {
            _EmptyContentLayout.gameObject.SetActive(false);
        }
    }
}
