using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using Unity.AI.Navigation;

public class Player : MonoBehaviour
{
    // キャラクターの移動に関するもの
    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector3 _moveDirection;
    private float _moveSpeed;
    public bool isRunnig = false;

    [SerializeField, Header("スタミナ減少率")] private float _consumeStamina = 0.1f;
    [SerializeField, Header("息切れ時停止時間")] private float _stopTime = 1.5f;
    [SerializeField, Header("プレイヤーの歩行速度")] private float _walkSpeed = 3f;
    [SerializeField, Header("プレイヤーの走る速度")] private float _runSpeed = 5f;
    [SerializeField, Header("無敵時間中の速度")] private float _speedInvincible = 6f;
    
    // 重力に関するパラメータ
    private Vector3 _velocity;
    [SerializeField, Header("重力の強さ")] private float gravity = -9.81f;
    
    // カメラとレイキャスト関連
    [SerializeField, Header("顔の前についているカメラ")] private Camera _camera;
    private Vector2 _lookInput;
    [SerializeField, Header("感度")] private float _lookSensitivity = 0.5f;
    [SerializeField, Header("上下で見れる角度")] private float _maxLookAngle = 60f;
    [SerializeField, Header("レイキャストの射程距離")] private float raycastDistance = 5f; 
    [SerializeField, Header("PlayerUIの中心のイメージ")] private Image _centerImage; 
    [SerializeField, Header("アイテムがない時の中心点")] private Sprite _normalCenter; 
    [SerializeField, Header("アイテムがある時の中心点")] private Sprite _itemCenter;
    [SerializeField, Header("敵の移動領域NavMesh")] private NavMeshSurface _navMeshSurface;
    private float _verticalRotation = 0f;

    // アニメーション
    private Animator _animator;

    // 音声に関するもの
    [SerializeField, Header("歩行音")] private AudioClip _walkSound;
    [SerializeField, Header("走る音")] private AudioClip _runSound;
    [SerializeField, Header("息切れ音")] private AudioClip _dushoutSound;
    [SerializeField, Header("ドアを開ける音")] private AudioClip _openDoor;
    private bool isPlayingWalkSound = false;
    private bool isPlayingRunSound = false;
    private PlayerHealth _playerHealth;
    private bool _isStopped = false;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerHealth = GetComponent<PlayerHealth>();
        _centerImage.sprite = _normalCenter;
    }
    /// <summary>
    /// 歩く状態,走る状態停止状態がある
    /// スタミナ0まで走り切った場合,息切れで一時停止する.
    /// </summary>
    async void Update()
    {
        if (_isStopped)
        {
            return;
        }
        else if (_playerHealth.GetStamina() <= 0)
        {
            _moveSpeed = 0f;
            _isStopped = true;
            SoundManager.Instance.PlaySE(_dushoutSound);
            await UniTask.Delay(TimeSpan.FromSeconds(_stopTime));
            SoundManager.Instance.StopSE();
            _isStopped = false;
        }
        else if (_playerHealth.isInvincible)
        {
            _moveSpeed = _speedInvincible;
        }
        else if (isRunnig && _playerHealth.GetStamina() > 0)
        {
            _moveSpeed = _runSpeed;
            _playerHealth.ConsumeStamina(_consumeStamina);
        }
        else
        {
            _moveSpeed = _walkSpeed;
        }

        // 前後の移動に限定
        _moveDirection = transform.forward * _moveInput.y;

        if (_moveDirection.magnitude > 0.1f)
        {
            if (_moveInput.y < 0)
            {
                transform.forward = -_moveDirection;
            }
            else
            {
                transform.forward = _moveDirection;
            }

            _animator.SetFloat("Speed", _moveSpeed);
        }
        else
        {
            _animator.SetFloat("Speed", 0);
            SoundManager.Instance.StopSE();
            isPlayingWalkSound = false;
            isPlayingRunSound = false;
        }

        if (_characterController.isGrounded)
        {
            _velocity.y = 0f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move((_moveDirection * _moveSpeed + _velocity) * Time.deltaTime);

        _centerImage.sprite = _normalCenter;
        ItemRay();

    }

    private void ItemRay()
    {
        RaycastHit hit;

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Item") || hit.collider.CompareTag("Door") || hit.collider.CompareTag("Lock"))
            {
                _centerImage.sprite = _itemCenter;
            }
        } 
    }
    
    /// <summary>
    /// 歩く,走る,見渡す,拾うのインプットアクションの処理
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                isRunnig = true;
                break;

            case InputActionPhase.Canceled:
                isRunnig = false;
                break;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();

        float horizontalRotation = _lookInput.x * _lookSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        _verticalRotation -= _lookInput.y * _lookSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxLookAngle, _maxLookAngle);

        _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Item"))
                {
                    Item item = hit.collider.GetComponent<Item>();
                    ReadBook book = hit.collider.GetComponent<ReadBook>();
                    if (item != null)
                    {
                        item.OnItemPickedUp();  
                    }
                    if (book != null)
                    {
                        book.OnBookRead().Forget();
                    }
                }
                else if(hit.collider.CompareTag("Door"))
                {
                    Animator _doorAnimator = hit.collider.GetComponent<Animator>();
                    _doorAnimator.SetTrigger("Open");
                    SoundManager.Instance.PlaySE3(_openDoor);
                    hit.collider.gameObject.tag = "Untagged";
                    //NavMeshBake
                    //_navMeshSurface.BuildNavMesh();

                }
                else if(hit.collider.CompareTag("Lock"))
                {
                    Lock _lock = hit.collider.GetComponent<Lock>();
                    _lock.ClearLock();
                }
            } 
        }
    }





    /// <summary>
    /// アニメーションイベントで歩行音を鳴らすためのメソッド
    /// </summary>
    public void WalkSound()
    {
        if (!isPlayingWalkSound)
        {
            SoundManager.Instance.StopSE();
            SoundManager.Instance.PlaySE(_walkSound);
            isPlayingWalkSound = true;
            isPlayingRunSound = false;
        }
    }

    public void RunSound()
    {
        if (!isPlayingRunSound)
        {
            SoundManager.Instance.StopSE();
            SoundManager.Instance.PlaySE(_runSound);
            isPlayingRunSound = true;
            isPlayingWalkSound = false;
        }
    }
}
