﻿using UnityEngine;
using Photon.Pun;

public class bl_ControllerExample : MonoBehaviourPunCallbacks
{
    [SerializeField] private bl_Joystick Joystick;
    [SerializeField] private float Speed = 5f;

    private Rigidbody2D rb;
    private PhotonView photonView;

    void Awake()
    {
        // 씬에서 조이스틱 찾아서 자동 할당
        if (Joystick == null)
        {
            Joystick = FindObjectOfType<bl_Joystick>();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();

        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // 혹시 조이스틱을 찾지 못했다면 다시 한번 시도
        if (Joystick == null)
        {
            Joystick = FindObjectOfType<bl_Joystick>();
            if (Joystick == null) return; // 조이스틱이 없으면 움직임 처리하지 않음
        }

        float horizontalInput = Joystick.Horizontal;
        float verticalInput = Joystick.Vertical;

        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        rb.linearVelocity = movement * Speed;
    }
}