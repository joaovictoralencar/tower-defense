using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Singletons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BuyDefensePreview : MonoBehaviour
{
    public LayerMask _layerToCheck;
    private Tower _towerPrefab;
    private Tower _tower;
    private GameObject _towerGFX;
    private Vector3 _mouseWorldPosition;
    private Vector3 _finalPosition;
    [SerializeField] private Transform _rangeTransform;
    [HideInInspector] public UnityEvent<Tower> OnBuy = new UnityEvent<Tower>();
    [HideInInspector] public UnityEvent<Tower> OnCancel = new UnityEvent<Tower>();

    private DefenseData _defenseData;

    public void Initialize(DefenseData defenseData)
    {
        _defenseData = defenseData;
        _towerPrefab = defenseData.prefab;
        _towerGFX = Instantiate(defenseData.prefab.GFX, transform);
        _towerGFXRenders = _towerGFX.GetComponentsInChildren<Renderer>();
        _rangeTransform.localScale = new Vector3(defenseData.prefab.AttackRange, 1, defenseData.prefab.AttackRange);
    }

    private CinemachineVirtualCamera _cinemachineCam;
    private Camera _cam;

    private Renderer[] _towerGFXRenders;

    private void Start()
    {
        _cinemachineCam = GameManager.Instance.MainVirtualCamera;
    }

    RaycastHit _cameraRayhit;

    private void Update()
    {
        if (_towerGFX == null) return;

        if (_cam == null)
        {
            _cam = Camera.main;
        }

        _canPlaceTower = false;

        Vector2 screenPosition = Input.mousePosition;
        _mouseWorldPosition = _cam.ScreenToWorldPoint(new Vector3
            (screenPosition.x, screenPosition.y, _cinemachineCam.m_Lens.FarClipPlane));
        Vector3 direction = _mouseWorldPosition - _cam.transform.position;

        if (Physics.Raycast(_cam.transform.position, direction, out _cameraRayhit, Mathf.Infinity, _layerToCheck))
        {
            if (!_cameraRayhit.collider.isTrigger)
            {
                _mouseWorldPosition = _cameraRayhit.point;
                if (_cameraRayhit.collider.CompareTag("NotWalkable"))
                {
                    _canPlaceTower = true;
                }

                Debug.Log(_cameraRayhit.collider);
            }
        }

        _finalPosition = _mouseWorldPosition;
        transform.position = _finalPosition;
        SetInvalidPositionMaterial(_canPlaceTower);

        if (_canPlaceTower && Input.GetMouseButtonDown(0))
        {
            BuyTower();
            _towerGFX = null;
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelBuy();
        }
    }

    private void BuyTower()
    {
        _tower = Instantiate(_towerPrefab, _finalPosition, Quaternion.identity);
        _tower.UpgradeCost = _defenseData.defenseCost / 1.5f;
        OnBuy?.Invoke(_tower);
        Destroy(gameObject);
    }

    private void CancelBuy()
    {
        OnCancel.Invoke(_tower);
        Destroy(gameObject);
    }

    private bool _canPlaceTower;
    private bool _lastIsValid;

    void SetInvalidPositionMaterial(bool newIsValid)
    {
        if (newIsValid == _lastIsValid) return;

        foreach (var render in _towerGFXRenders)
        {
            render.material.SetColor("_BaseColor", newIsValid ? new Color(0, 1, 0, .5f) : new Color(1, 0, 0, .5f));
        }

        _canPlaceTower = newIsValid;
        _lastIsValid = _canPlaceTower;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_finalPosition, 1);
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(_mouseWorldPosition, 1);
    }
#endif
}