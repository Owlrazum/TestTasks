using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

using TMPro;

public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Color _defaultBack = Color.white;

    [SerializeField]
    private Color _highlightBack = Color.black;

    [SerializeField]
    private Color _defaultText = Color.black;

    [SerializeField]
    private Color _highlightText = Color.white;

    [SerializeField]
    private Color _notInteractable = Color.grey;

    private Image _back;
    private TextMeshProUGUI _textMesh;

    private Material cachedBackMaterial;

    private void Awake()
    {
        bool isFound = transform.GetChild(0).TryGetComponent(out _textMesh);
        isFound &= TryGetComponent(out _back);
        Assert.IsTrue(isFound);

        cachedBackMaterial = Instantiate(_back.material);
        _back.material = cachedBackMaterial;

        _canInteract = true;
    }

    private bool _canInteract;
    public bool CanInteract
    {
        get { return _canInteract; }
        set 
        { 
            _canInteract = value;
            if (!_canInteract)
            { 
                _back.material.color = _notInteractable;
            }
            else
            {
                _back.material.color = _defaultBack;
            }
        }
    }
    public Action BeforeOnClick { get; set; }
    public Action OnClick { get; set; }

    public void ChangeButtonText(string buttonText)
    {
        _textMesh.text = buttonText;
    }
    
    public void OnPointerClick(PointerEventData data)
    {
        if (!_canInteract || !gameObject.activeSelf)
        {
            return;
        }

        BeforeOnClick?.Invoke();
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (!_canInteract || !gameObject.activeSelf)
        {
            return;
        }

        _back.material.color = _highlightBack;
        _textMesh.color = _highlightText;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!_canInteract || !gameObject.activeSelf)
        {
            return;
        }

        _back.material.color = _defaultBack;
        _textMesh.color = _defaultText;
    }

    public void Show()
    {
        Debug.Log("Show start button");
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("Hide start button");
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(cachedBackMaterial);
    }
}