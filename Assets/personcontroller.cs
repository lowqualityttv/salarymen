using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class personcontroller : MonoBehaviour
{
    public float speed = 1;
    public float postCoffeeUseSpeedScalar = 2;

    float randomWalkDistance = 0;

    public SpriteRenderer smoke = null;
    [SerializeField] private Sprite smoking1Sprite = null;
    [SerializeField] private Sprite smoking2Sprite = null;
    [SerializeField] private Sprite smoking3Sprite = null;

    [SerializeField] private Sprite smokingSprite = null;
    [SerializeField] private Transform smokeSystem = null;

    public SpriteRenderer coffee = null;
    [SerializeField] private Sprite coffee1Sprite = null;
    [SerializeField] private Sprite coffee2Sprite = null;
    [SerializeField] private Sprite coffee3Sprite = null;

    [SerializeField] private Sprite coffeeSprite = null;

    [SerializeField] private Animator animator = null;

    private Vector3 startPosition;

    bool acting = false;

    public int requiredClicks = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<SpriteRenderer>().flipX)
        {
            speed = -speed;
            smokeSystem.localPosition = new Vector3(-smokeSystem.localPosition.x, smokeSystem.localPosition.y, smokeSystem.localPosition.z);
        }

        startPosition = transform.position;

        randomWalkDistance = Random.Range(2, 15);
        StartCoroutine(DoActionCoroutine());
    }

    private IEnumerator DoActionCoroutine()
    {
        yield return new WaitUntil(() => Vector3.Distance(startPosition, transform.position) > randomWalkDistance);
        if (Random.Range(0, 2) == 0)
        {
            smoke.gameObject.SetActive(true);
        }
        else
        {
            coffee.gameObject.SetActive(true);
        }

        if (Random.Range(0, 5) == 0)
        {
            requiredClicks = 2;
        }

        if (Random.Range(0, 10) == 0)
        {
            requiredClicks = 3;
        }

        if (requiredClicks == 3)
        {
            smoke.sprite = smoking3Sprite;
            coffee.sprite = coffee3Sprite;
        }
        else if (requiredClicks == 2)
        {
            smoke.sprite = smoking2Sprite;
            coffee.sprite = coffee2Sprite;
        }
        else if (requiredClicks == 1)
        {
            smoke.sprite = smoking1Sprite;
            coffee.sprite = coffee1Sprite;
        }

        //if (requiredClicks > 1)
        //{
        //comboText.gameObject.SetActive(true);
        //comboText.text = "x" + requiredClicks;
        //}
    }

    public void StartDrinkingCoffee()
    {
        acting = true;
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = coffeeSprite;
        if (stopdrinkingcoffee != null)
        {
            StopCoroutine(stopdrinkingcoffee);
        }
        stopdrinkingcoffee = StartCoroutine(StopDrinkingCoffee());
    }

    private Coroutine stopdrinkingcoffee = null;
    private Coroutine stopsmoking = null;

    private IEnumerator StopDrinkingCoffee()
    {
        yield return new WaitForSeconds(3f);
        acting = false;
        animator.enabled = true;
        speed *= postCoffeeUseSpeedScalar;
        stopdrinkingcoffee = null;
    }

    public void StartSmoking()
    {
        if (!personmaker.Instance.ecigActive)
        {
            smokeSystem.GetComponent<ParticleSystem>().enableEmission = true;
        }
        acting = true;
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = smokingSprite;
        if (stopsmoking != null)
        {
            StopCoroutine(stopsmoking);
        }
        stopsmoking = StartCoroutine(StopSmoking());
    }

    private IEnumerator StopSmoking()
    {
        yield return new WaitForSeconds(3f);
        smokeSystem.GetComponent<ParticleSystem>().enableEmission = false;
        acting = false;
        animator.enabled = true;
        speed *= 2;
    }

    public void UpdateClicks()
    {
        if (requiredClicks > 0)
        {
            requiredClicks--;
            if (requiredClicks == 3)
            {
                smoke.sprite = smoking3Sprite;
                coffee.sprite = coffee3Sprite;
            }
            else if (requiredClicks == 2)
            {
                smoke.sprite = smoking2Sprite;
                coffee.sprite = coffee2Sprite;
            }
            else if (requiredClicks == 1)
            {
                smoke.sprite = smoking1Sprite;
                coffee.sprite = coffee1Sprite;
            }

        }
    }

    public void InteractWithItem(GameObject item)
    {
        if (requiredClicks <= 0)
        {
            if (item.tag == "Cigs")
            {
                smoke.gameObject.SetActive(false);
                StartSmoking();
            }
            else if (item.tag == "Coffee")
            {
                coffee.gameObject.SetActive(false);
                StartDrinkingCoffee();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (acting)
        {
            return;
        }
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime * -speed;

        if (transform.position.x > 11 || transform.position.x < -11)
        {
            personmaker.Instance.RemoveSalaryman(gameObject);
            Destroy(gameObject);
        }
    }
}
