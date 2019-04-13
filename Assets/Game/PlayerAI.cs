using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour {
    public float speed = 2;
    public Rect bounds;
    public GameObject rotationObject;
    public GameObject arrowPrefab;
    public float arrowDelay = .5f;
    private float timeBeforeNextArrow = 0;
    public float respawnTime = 3;

    public AoeAI aoe;
    public GameObject invincibility;

    public float invincibilityDuration = 1;
    public float spawnInvincibilityDuration = 1;
    public float invincibilityCooldown = 20;
    private float invincibilityRemaining = 0;
    private float timeBeforeNextInvincibility = 0;

    public float aoeDuration = 3;
    public float aoeTimeToKill = 0.75f;
    public float aoeCooldown = 15;
    private float aoeRemaining = 0;
    public float aoeHealSpeed = 0.5f;
    private float timeBeforeNextAoe = 0;
    
    private float insideAoeDuration = 0;

    public bool dead = false;
    public int score = 0;
    public Color color;
    public int index = 0;

    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    public IntEvent onScoreChanged;

	// Use this for initialization
	void Start ()
    {
        color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        GetComponentInChildren<SpriteRenderer>().color = color;
        gameObject.layer = LayerMask.NameToLayer($"Player{index}");
        aoe.player = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (timeBeforeNextArrow > 0)
            timeBeforeNextArrow -= Time.deltaTime;
        if (timeBeforeNextInvincibility > 0)
            timeBeforeNextInvincibility -= Time.deltaTime;
        if (timeBeforeNextAoe > 0)
            timeBeforeNextAoe -= Time.deltaTime;

        if (dead)
            return;

        insideAoeDuration -= Time.deltaTime * aoeHealSpeed;
        if (insideAoeDuration < 0)
            insideAoeDuration = 0;

        if (invincibilityRemaining > 0)
        {
            invincibilityRemaining -= Time.deltaTime;
            if (invincibilityRemaining <= 0)
                EndInvincibility();
        }

        if (aoeRemaining > 0)
        {
            aoeRemaining -= Time.deltaTime;
            if (aoeRemaining <= 0)
                EndAoe();
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical).normalized * Math.Max(Math.Abs(horizontal), Math.Abs(vertical)) * speed * Time.deltaTime;

        gameObject.transform.Translate(move.x, move.y, 0);

        if (move != Vector2.zero)
        {
            rotationObject.transform.eulerAngles = (Vector3.forward * Vector2.SignedAngle(Vector2.up, move));
        }
        if (Input.GetButton("Fire1") && timeBeforeNextArrow <= 0)
        {
            ShootArrow();
        }
        if (Input.GetButton("Fire2") && timeBeforeNextInvincibility <= 0)
        {
            StartInvincibility();
        }
        if (Input.GetButton("Fire3") && timeBeforeNextAoe <= 0)
        {
            StartAoe();
        }
    }

    public void StartInvincibility()
    {
        invincibilityRemaining = invincibilityDuration;
        timeBeforeNextInvincibility = invincibilityCooldown;
        invincibility.SetActive(true);
    }

    public void EndInvincibility()
    {
        invincibilityRemaining = 0;
        invincibility.SetActive(false);
    }

    public void StartAoe()
    {
        aoeRemaining = aoeDuration;
        timeBeforeNextAoe = aoeCooldown;
        aoe.gameObject.SetActive(true);
    }

    public void EndAoe()
    {
        aoeRemaining = 0;
        aoe.gameObject.SetActive(false);
    }

    private void ShootArrow()
    {
        timeBeforeNextArrow = arrowDelay;
        var arrow = Instantiate(arrowPrefab, rotationObject.transform.position, rotationObject.transform.rotation);
        arrow.GetComponent<ArrowAI>().player = this;
        arrow.GetComponent<SpriteRenderer>().color = color;
        IncrementScore(-1);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<AoeAI>() != null)
        {
            if (invincibilityRemaining > 0)
            {
                insideAoeDuration = 0;
            }
            else
            {
                insideAoeDuration += Time.deltaTime;
                if (insideAoeDuration >= aoeTimeToKill)
                    KillBy(collision.GetComponent<AoeAI>().player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead)
            return;
        if (collision.GetComponent<ArrowAI>() != null)
        {
            KillBy(collision.GetComponent<ArrowAI>().player);
            Destroy(collision.gameObject);
        }
        var killer = collision.gameObject.GetComponent<ArrowAI>().player;
    }

    private void KillBy(PlayerAI killer)
    {
        if (invincibilityRemaining > 0)
            return;
        IncrementScore(-10);
        StartCoroutine(Reactivate());
        rotationObject.SetActive(false);
        EndInvincibility();
        invincibilityRemaining = spawnInvincibilityDuration;
        insideAoeDuration = 0;
        killer.IncrementScore(20);
        dead = true;
    }

    private void IncrementScore(int increment)
    {
        score += increment;
        onScoreChanged.Invoke(score);
    }

    private IEnumerator Reactivate()
    {
        yield return new WaitForSeconds(respawnTime);
        if (dead)
        {
            rotationObject.SetActive(true);
            dead = false;
        }
    }

    public void EndGame()
    {
        timeBeforeNextArrow = 0;
        EndInvincibility();
        EndAoe();
        timeBeforeNextInvincibility = 0;
        timeBeforeNextAoe = 0;
        insideAoeDuration = 0;
        StopAllCoroutines();
        rotationObject.SetActive(true);
        dead = false;
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        gameObject.SetActive(true);
    }
}
