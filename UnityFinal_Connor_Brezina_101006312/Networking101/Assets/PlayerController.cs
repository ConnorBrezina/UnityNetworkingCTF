﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomMsgType
{
    public static short Transform = MsgType.Highest + 1;
};


public class PlayerController : NetworkBehaviour
{
    public float m_linearSpeed = 5.0f;
    public float m_angularSpeed = 3.0f;
    public float m_jumpSpeed = 5.0f;
	public float score = 0;

    private Rigidbody m_rb = null;

    [SyncVar]
    private bool m_hasFlag = false;
	public static float ScoreP1;
	public static float ScoreP2;
    public bool HasFlag() {
        return m_hasFlag;
    }

    [Command]
    public void CmdPickUpFlag()
    {
        m_hasFlag = true;
    }

    [Command]
    public void CmdDropFlag()
    {
        m_hasFlag = false;
    }



    bool IsHost()
    {
        return isServer && isLocalPlayer;
    }

    // Use this for initialization
    void Start() {
		
        m_rb = GetComponent<Rigidbody>();
        //Debug.Log("Start()");
        Vector3 spawnPoint;
        ObjectSpawner.RandomPoint(this.transform.position, 50.0f, out spawnPoint);
        this.transform.position = spawnPoint;

        TrailRenderer tr = GetComponent<TrailRenderer>();
        tr.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        //Debug.Log("OnStartAuthority()");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log("OnStartClient()");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //Debug.Log("OnStartLocalPlayer()");
        GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //Debug.Log("OnStartServer()");
    }

    public void Jump()
    {
        Vector3 jumpVelocity = Vector3.up * m_jumpSpeed;
        m_rb.velocity += jumpVelocity;
        TrailRenderer tr = GetComponent<TrailRenderer>();
        tr.enabled = true;
    }

    [ClientRpc]
    public void RpcJump()
    {
        Jump();
    }

    [Command]
    public void CmdJump()
    {
        Jump();
        RpcJump();
    }

    // Update is called once per frame
    void Update() {
		if (!isLocalPlayer) {
			ScoreP1 = score;
		} else {
			ScoreP2 = score;
		}
		Transform childTran = this.transform.GetChild(this.transform.childCount - 1);
		Flag flag = childTran.gameObject.GetComponent<Flag>();
		if (flag) {
			score += Time.deltaTime;
		}
        if (!isLocalPlayer)
        {
            return;
        }

        if (m_rb.velocity.y < Mathf.Epsilon) {
            TrailRenderer tr = GetComponent<TrailRenderer>();
            tr.enabled = false;
        }

        float rotationInput = Input.GetAxis("Horizontal");
        float forwardInput = Input.GetAxis("Vertical");

        Vector3 linearVelocity = this.transform.forward * (forwardInput * m_linearSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdJump();
        }

        float yVelocity = m_rb.velocity.y;


        linearVelocity.y = yVelocity;
        m_rb.velocity = linearVelocity;

        Vector3 angularVelocity = this.transform.up * (rotationInput * m_angularSpeed);
        m_rb.angularVelocity = angularVelocity;
    }

    [Command]
    public void CmdPlayerDropFlag()
    {
        Transform childTran = this.transform.GetChild(this.transform.childCount - 1);
        Flag flag = childTran.gameObject.GetComponent<Flag>();
		if (flag) {
			flag.CmdDropFlag();
		}
    }
	public void OnCollisionEnter(Collision other)
	{
		if(!isLocalPlayer || other.collider.tag != "Player" || other.collider.tag != "P1")
		{
			//return;
		}
		if (other.collider.tag == "P1") {
			Destroy (other.gameObject);
			StartCoroutine (Speed ());
		}
		if (other.collider.tag == "P2") {
		}
			
		if (HasFlag()) {
			Transform childTran = this.transform.GetChild (this.transform.childCount - 1);
			if (childTran.gameObject.tag == "Flag") {
                CmdPlayerDropFlag();
            }
		}
	}
	IEnumerator Speed()
	{
		m_linearSpeed += 20.0f;
		yield return new WaitForSeconds (3f);
		m_linearSpeed -= 20.0f;
	}

}
