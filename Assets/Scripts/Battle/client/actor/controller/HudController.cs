#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HudController.cs
 Author:      Zeng Zhiwei
 Time:        2020\11\28 星期六 16:57:15
=====================================================
*/
#endregion

using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Slider HpSlider;

    private Camera _uiCamera;
    private float _barOffset;
    private bool _isFriendly;
    private Camera _worldCamera;
    private RectTransform _cacheTransfrom;
    private Transform _targetTransform;
    private RectTransform _canvasTransfrom;

    private void Awake()
    {
        _cacheTransfrom = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        //if(_hpAnimTweenId != 0)
        //{
        //    LeanTween.cancel(_hpAnimTweenId);
        //}
    }

    public void Init(RectTransform canvasTransform, Transform targetTransform, Camera uiCamera, float barOffset, bool isFriendly, float barShowDuration = 0f)
    {
        _worldCamera = Camera.main;// todo camera controller
        _uiCamera = uiCamera;
        _barOffset = barOffset;
        _isFriendly = isFriendly;
        _canvasTransfrom = canvasTransform;
        _targetTransform = targetTransform;
    }

    private void Update()
    {
        if(_targetTransform == null)
            return;

        _cacheTransfrom.anchoredPosition = World2RectPosition();

        //if(_state == BloodBarState.INACTIVE)
        //{
        //    _showedTime += Time.deltaTime;
        //    if(_showedTime >= _barShowDuration)
        //    {
        //        HideBloodBar();
        //    }
        //}
    }

    // 世界坐标转屏幕坐标
    private Vector2 World2RectPosition()
    {
        Vector3 targetPosition = _targetTransform.position;
        targetPosition.y += _barOffset;
        var sceenPoint = _worldCamera.WorldToScreenPoint(targetPosition);

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransfrom, sceenPoint, _uiCamera, out anchoredPosition);
        return anchoredPosition;
    }

    public void SetValue(float value)
    {
        HpSlider.value = value;
        //float lastValue = HpAnimImage.fillAmount;

        //if(_hpAnimTweenId != 0)
        //{
        //    LeanTween.cancel(_hpAnimTweenId);
        //}

        //HpSlider.gameObject.SetActive(true);
        //_hpAnimTweenId = LeanTween.value(lastValue, value, 0.3f).setOnUpdate(OnHpAnimUpdate).id;
    }
}