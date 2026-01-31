using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private Camera _mainCamera;
    public IObservable<(GameObject obj, Vector3 screenPos)> OnTapped => _onTapped;
    private readonly Subject<(GameObject, Vector3)> _onTapped = new Subject<(GameObject, Vector3)>();
    public IObservable<Vector3?> MousePosAsObservable => _mousePos;
    private readonly Subject<Vector3?> _mousePos = new Subject<Vector3?>();
    private bool _isTapping = false;

    protected override void Awake()
    {
        if (!CheckInstance())
        {
            return;
        }
        _playerAction = new PlayerAction();
        OnEnable();
    }
    /// <summary>
    /// 有効化
    /// </summary>
    private void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.PlayerTouch.Tap.started += OnTapStarted;
    }
    /// <summary>
    /// 無効化
    /// </summary>
    private void OnDisable()
    {
        _playerAction.PlayerTouch.Tap.started -= OnTapStarted;
        _playerAction.Disable();
    }
    /// <summary>
    /// タップ関連
    /// </summary>
    /// <param name="context"></param>
    private void OnTapStarted(InputAction.CallbackContext context)
    {
        TapEvent(context);
        _isTapping = true;
    }
    private void Update()
    {
        if (_isTapping)
        {
            var tmpScreenPos = GetDeviceValue();
            if (tmpScreenPos == null)
            {
                return;
            }

            Vector3 screenPos = tmpScreenPos.Value;
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                _mousePos.OnNext(hit.point);
            }
        }
    }

    private void TapEvent(InputAction.CallbackContext context)
    {
        var tmpScreenPos = GetDeviceValue();
        if (tmpScreenPos == null) return;
        Vector3 screenPos = tmpScreenPos.Value;
        screenPos.z = _mainCamera.nearClipPlane;

        //UnityのWorldPositionに置き換え
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);

        Vector3 direction = (worldPos - _mainCamera.transform.position).normalized;

        //オブジェクトの取得
        List<GameObject> hitObjs = Raycast3D(worldPos, direction, "Animal");
        if (hitObjs != null && hitObjs.Count > 0)
        {
            _onTapped.OnNext((hitObjs[0], screenPos));
        }
        else
        {
            Debug.Log("No Hit Object");
        }

    }
    /// <summary>
    /// デバイスに応じた座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3? GetDeviceValue()
    {
        //マウスが接続ならマウス座標を返す
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        //指の座標を返す
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.touches.Count == 1)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }
        }
        //例外
        return null;
    }
    /// <summary>
    /// Raycastを使い、条件に合うGameObjectを返す
    /// </summary>
    /// <param name="origin">Raycastの開始地点</param>
    /// <param name="direction">Raycastの向き</param>
    /// <param name="targetTag">目的のタグ</param>
    /// <param name="distance">Raycastの長さ</param>
    /// <returns></returns>
    private List<GameObject> Raycast3D(
         Vector3 origin,
         Vector3 direction,
         string targetTag = null,
         float distance = Mathf.Infinity)
    {
        //nullフラグ
        bool isTagNull = string.IsNullOrEmpty(targetTag);
        //ターゲットのTagとLayer番号が指定されていない場合中断
        if (isTagNull) return null;
        RaycastHit[] hitObjs = Physics.RaycastAll(origin, direction, distance);
        Debug.DrawRay(origin, direction, Color.red, 1f);
        if (hitObjs == null || hitObjs.Length == 0) return null;
        RaycastHit[] sortedHits = hitObjs.OrderBy(hit => hit.distance).ToArray();
        List<GameObject> hisObjs = new List<GameObject>();
        foreach (var hitObj in sortedHits)
        {
            if (hitObj.collider == null) continue;
            //tagがnullかタグが合致する
            bool isTagMatch = isTagNull || hitObj.collider.tag == targetTag;
            //タグかレイヤーがマッチしたオブジェクトを返す
            if (isTagMatch)
            {
                hisObjs.Add(hitObj.collider.gameObject);
            }
        }
        return hisObjs;
    }
}
