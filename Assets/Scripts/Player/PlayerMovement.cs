﻿using System;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Cinemachine;

namespace Player
{
    public class PlayerMovement : MonoSingleton<PlayerMovement>
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform playerBody;
        
        private Rigidbody2D rb;

        private CinemachineTransposer vCamTransposer;

        private float lerpT;
        private float lerpFrom;
        private float lerpTo;
        private bool isLerping;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            vCamTransposer = VCamInstance.Current.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void Update()
        {
            Move();
            Jump();
        }

        private void Move()
        {
            var horizontal = Input.GetAxis("Horizontal");
            // rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

            if (horizontal != 0)
            {
                playerBody.localRotation = Quaternion.Euler(new Vector3(playerBody.localRotation.eulerAngles.x,
                    horizontal > 0 ? 0 : 180, playerBody.localRotation.eulerAngles.z));

                if (!isLerping)
                {
                    lerpFrom = vCamTransposer.m_FollowOffset.x;
                    lerpTo = Mathf.Sign(horizontal) * 3.5f;
                    lerpT = 0;
                    isLerping = true;
                }
                
                var offset = vCamTransposer.m_FollowOffset;
                offset.x = Mathf.Lerp(lerpFrom, lerpTo, Mathf.SmoothStep(0, 1, lerpT));
                vCamTransposer.m_FollowOffset = offset;

                lerpT += Time.deltaTime * 1.2f;
            }
            else
            {
                isLerping = false;
            }
        }

        private void Jump()
        {
            if (!Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.Space)) return;

            var isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
            if (!isGrounded) return;

            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }
}
