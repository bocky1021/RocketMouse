using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MouseController : MonoBehaviour
{
    public float jetpackForce;
    public ParticleSystem jetpack;

    public float forwardMovementSpeed;

    private Rigidbody2D rb;

    public Transform groundCheckTransform;
    public LayerMask groundCheckLayerMask;
    private bool grounded;
    private Animator animator;

    private bool dead = false;

    private uint coins = 0;
    public TMP_Text textCoins;

    public GameObject buttonRestart;
    public GameObject buttonGoMenu;

    public AudioClip coinCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    public AudioSource bgMusicAudio;

    public ParallaxScroll parallaxScroll;

    private int lv;
    public float lvUpInterval;
    private float lvUpTimeCnt;
    public TMP_Text textLv;

    public bool isFever;
    public float feverInterval;
    public float feverTime;

    Coroutine feverCoroutine;

    public int lifeCnt;
    private float invincibleTimeCnt;

    public SpriteRenderer sp;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        textCoins.text = coins.ToString();

        lv = 1;
        textLv.text = $"Lv.{lv}";

        LoadVolume();

        feverCoroutine = StartCoroutine(FeverCtrl());
    }

    private void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        if (!dead)
        {
            if (jetpackActive)
            {
                rb.AddForce(jetpackForce * Vector2.up);
            }

            Vector2 newVelocity = rb.velocity;
            newVelocity.x = forwardMovementSpeed;
            rb.velocity = newVelocity;
        }

        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        DisplayButtons();
        AdjustFootstepsAndJetpackSound(jetpackActive);

        parallaxScroll.offset = transform.position.x;
    }

    private void Update()
    {
        if (dead)
            return;

        lvUpTimeCnt += Time.deltaTime;
        if (lvUpTimeCnt >= lvUpInterval)
        {
            lv++;
            textLv.text = $"Lv.{lv}";
            forwardMovementSpeed = 2.5f + lv * 0.5f;

            lvUpTimeCnt = 0;
        }

        if (invincibleTimeCnt > 0)
            invincibleTimeCnt -= Time.deltaTime;
    }

    private void AdjustJetpack(bool jetpackActive)
    {
        var emission = jetpack.emission;
        emission.enabled = !grounded;
        emission.rateOverTime = jetpackActive ? 300f : 75f;
    }

    private void UpdateGroundedStatus()
    {
        grounded = Physics2D.OverlapCircle(
            groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        animator.SetBool("grounded", grounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coins")
            CollectCoin(collision);
        else
            HitByLaser(collision);
    }

    private void CollectCoin(Collider2D coinCollider)
    {
        ++coins;
        textCoins.text = coins.ToString();

        Destroy(coinCollider.gameObject);

        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }

    private void HitByLaser(Collider2D laserCollider)
    {
        if (invincibleTimeCnt > 0)
            return;

        if (!dead)
        {
            AudioSource laser = laserCollider.GetComponent<AudioSource>();
            laser.Play();
        }

        --lifeCnt;
        if (lifeCnt > 0)
        {
            invincibleTimeCnt = 3f;
            StartCoroutine(InvincibleTime());
            return;
        }

        dead = true;
        animator.SetBool("dead", true);

        StopCoroutine(feverCoroutine);
    }

    private void DisplayButtons()
    {
        bool active = buttonRestart.activeSelf;
        if (grounded && dead && !active)
        {
            buttonRestart.SetActive(true);
            buttonGoMenu.SetActive(true);
        }
    }

    public void OnClickedRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickedGoMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    private void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !dead && grounded;
        jetpackAudio.enabled = !dead && !grounded;
        jetpackAudio.volume = jetpackActive ? 1f : 0.5f;
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("bgVolume");
        bgMusicAudio.volume = volume;
    }

    IEnumerator InvincibleTime()
    {
        for (int i = 0; i < 3; i++)
        {
            sp.color = new Color(0.5f, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            sp.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FeverCtrl()
    {
        while (true)
        {
            yield return new WaitForSeconds(feverInterval);

            isFever = true;
            forwardMovementSpeed = 10f;
            GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
            foreach (var obj in lasers)
                obj.SetActive(false);

            yield return new WaitForSeconds(feverTime);

            isFever = false;
            forwardMovementSpeed = 2.5f + lv * 0.5f;
        }
    }
}
