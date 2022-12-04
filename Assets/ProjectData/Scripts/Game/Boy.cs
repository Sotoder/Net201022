using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using PlayFab;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Boy: MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] protected CharacterController _characterController;
    [SerializeField] protected Camera _camera;
    [SerializeField] protected AudioListener _listner;
    [SerializeField] protected PhotonView _photonView;
    [SerializeField] protected Transform _shootingPoint;
    [SerializeField] protected LayerMask _playerLayer;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _sensetivity;

    [SerializeField] protected float _hp;
    [SerializeField] protected float _dmg;
    protected bool _isShoot;

    public PhotonView PhotonView => _photonView;

    protected void Start()
    {
        if(!_photonView.IsMine)
        {
            _camera.enabled = false;
            _listner.enabled = false;
        }
    }

    protected void Update()
    {
        if(_photonView.IsMine) 
        {
            Move();
            Attack();
        }
    }

    protected void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            Rotate();
        }
    }

    protected void Rotate()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * _sensetivity * Time.fixedDeltaTime, 0);
    }

    protected void Move()
    {

        var move = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

        _characterController.Move(move * Time.deltaTime * _speed);
    }
    protected void GetDamage(float damage)
    {
        _hp -= damage;
        SendHpParameter(_photonView.ViewID, _hp);
    }

    protected void Attack()
    {
        if (_photonView.IsMine)
        {

            if (Input.GetKey(KeyCode.Mouse0) && !_isShoot)
            {
                RaycastHit hit;

                if (Physics.Raycast(_shootingPoint.position, _shootingPoint.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, _playerLayer))
                {
                    var damagedPlayerID = hit.collider.gameObject.GetComponent<PhotonView>().ViewID;
                    SendDamageEvent(damagedPlayerID, _dmg);
                }

                StartShootAnimation();
                _isShoot = true;
                SendAttackEvent(_photonView.ViewID);
            }
            else if (!Input.GetKey(KeyCode.Mouse0) && _isShoot)
            {
                _isShoot = false;
            }
        }
    }

    protected abstract void StartShootAnimation();

    protected void SendAttackEvent(int viewID)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] eventContent = new object[]
        {
            viewID,
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventsTypes.ShootEvent, eventContent, options, sendOptions);
        Debug.Log("SendShootEvent");
    }

    protected void SendDamageEvent(int damagedPlayerID, float dmg)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] eventContent = new object[]
        {
            damagedPlayerID,
            dmg
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventsTypes.DamageEvent, eventContent, options, sendOptions);
        Debug.Log("SendDamageEvent");
    }

    private void SendHpParameter(int viewID, float hp)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] eventContent = new object[]
        {
            viewID,
            hp
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventsTypes.ChangeHpEvent, eventContent, options, sendOptions);
        Debug.Log("SendHPEvent");
    }


    public void OnEvent(EventData photonEvent)
    {
        switch ((EventsTypes)photonEvent.Code)
        {
            case EventsTypes.ParametersSyncEvent:

                object[] parametersSyncData = (object[])photonEvent.CustomData;

                if(_photonView.ViewID == (int)parametersSyncData[0])
                {
                    _hp = (float)parametersSyncData[1];
                    _dmg = (float)parametersSyncData[2];
                }
                break;

            case EventsTypes.ShootEvent:

                object[] shootEventData = (object[])photonEvent.CustomData;

                if (_photonView.ViewID == (int)shootEventData[0])
                {
                    StartShootAnimation();
                }
                break;

            case EventsTypes.DamageEvent:

                object[] damageEventData = (object[])photonEvent.CustomData;

                if (_photonView.ViewID == (int)damageEventData[0])
                {
                    GetDamage((float)damageEventData[1]);
                }
                break;

            case EventsTypes.ChangeHpEvent:

                object[] hpEventData = (object[])photonEvent.CustomData;

                if (_photonView.ViewID == (int)hpEventData[0])
                {
                    _hp = (float)hpEventData[1];
                }
                break;

            default:
                break;
        }
    }

    public void SetCharacterParameters(float hp, float damage)
    {
        _hp = hp;
        _dmg = damage;
    }

}