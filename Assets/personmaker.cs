using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;

public class personmaker : AbstractManager<personmaker>
{
    //coffee perks make people walk faster after drinking (clears screen)
    //blunderbus (3 cigs at once)

    [SerializeField] private GameObject personPrefab = null;

    private float personTimer = 0;
    private float personDelay = 5;

    [SerializeField] private GameObject bossCoffee = null;
    [SerializeField] private GameObject cigarettes = null;

    [SerializeField] private Transform coots = null;

    [SerializeField] private AnimationCurve curve = null;

    [SerializeField] private Sprite cootsSprite = null;
    [SerializeField] private Sprite cootsThrowSprite = null;

    [SerializeField] private GameObject coinPrefab = null;

    [SerializeField] private TextMeshProUGUI timerText = null;
    [SerializeField] private TextMeshProUGUI lvlText = null;

    private float experienceToLevel = 5;
    private float experienceAdded2to20 = 10;
    private float experienceAdded20to40 = 13;
    private float experienceAdded40 = 16;

    private int experience = 0;
    private int level = 1;

    [SerializeField] private RectTransform levelUpBar = null;

    [SerializeField] private AudioSource audioSource = null;

    [SerializeField] private AudioClip catMeow = null;
    [SerializeField] private AudioClip catThrow = null;
    [SerializeField] private AudioClip xpCollection = null;

    [SerializeField] private GameObject levelUpMenu = null;
    [SerializeField] private Transform levelUpMenuContent = null;

    [SerializeField] private GameObject[] itemPrefabs = null;

    private List<GameObject> salarymen = new List<GameObject>();

    [SerializeField] private GameObject ders = null;
    [SerializeField] private GameObject swift = null;

    private int coffeeFires = 1;
    private int cigFires = 1;

    private float dersTimer = 3;
    private float swiftTimer = 3;

    private float postItemUseSpeedScalar = 2;

    private void Start()
    {
        //Time.timeScale = 5;
    }

    public void RemoveSalaryman(GameObject man)
    {
        salarymen.Remove(man);
    }

    private void Update()
    {
        //if (isPaused)
        //{
        //    return;
        //}

        if (swift.activeInHierarchy)
        {
            swiftTimer -= Time.deltaTime;
            if (swiftTimer <= 0)
            {
                for (int i = 0; i < salarymen.Count; i++)
                {
                    if (salarymen[i].GetComponent<personcontroller>().coffee.gameObject.activeInHierarchy)
                    {
                        salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                        GameObject coffee = Instantiate(bossCoffee, swift.transform.position, Quaternion.identity);
                        StartCoroutine(SendItemFlying(coffee, salarymen[i].transform, false));
                        //StartCoroutine(ThrowItem());
                        swiftTimer = 3;

                        break;
                    }
                }
            }
        }

        if (ders.activeInHierarchy)
        {
            dersTimer -= Time.deltaTime;
            if (dersTimer <= 0)
            {
                for (int i = 0; i < salarymen.Count; i++)
                {
                    if (salarymen[i].GetComponent<personcontroller>().smoke.gameObject.activeInHierarchy)
                    {
                        salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                        GameObject cigs = Instantiate(cigarettes, ders.transform.position, Quaternion.identity);
                        StartCoroutine(SendItemFlying(cigs, salarymen[i].transform, false));
                        //StartCoroutine(ThrowItem());
                        dersTimer = 3;

                        break;
                    }
                }
            }
        }

        System.TimeSpan TimeInSeconds = System.TimeSpan.FromSeconds(((double)Time.timeSinceLevelLoad));

        timerText.text = TimeInSeconds.ToString(@"mm\:ss");

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero);

            if (hit.collider != null && hit.collider.tag == "Cigs")
            {
                if (hit.collider.transform.parent.GetComponent<personcontroller>().requiredClicks > 0)
                {
                    hit.collider.transform.parent.GetComponent<personcontroller>().UpdateClicks();

                    GameObject cigs = Instantiate(cigarettes, coots.transform.position, Quaternion.identity);
                    StartCoroutine(SendItemFlying(cigs, hit.collider.transform.parent));
                    StartCoroutine(ThrowItem());

                    int cigClicks = cigFires - 1;

                    for (int i = 0; i < salarymen.Count; i++)
                    {
                        if (cigClicks > 0)
                        {
                            if (salarymen[i] != hit.collider.transform.parent.gameObject)
                            {
                                if (salarymen[i].GetComponent<personcontroller>().smoke.gameObject.activeInHierarchy)
                                {
                                    salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                                    cigs = Instantiate(cigarettes, coots.transform.position, Quaternion.identity);
                                    StartCoroutine(SendItemFlying(cigs, salarymen[i].transform, false));
                                    StartCoroutine(ThrowItem());
                                    cigClicks--;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero);

            if (hit.collider != null && hit.collider.tag == "Coffee")
            {
                if (hit.collider.transform.parent.GetComponent<personcontroller>().requiredClicks > 0)
                {
                    hit.collider.transform.parent.GetComponent<personcontroller>().UpdateClicks();
                    GameObject coffee = Instantiate(bossCoffee, coots.transform.position, Quaternion.identity);
                    StartCoroutine(SendItemFlying(coffee, hit.collider.transform.parent));
                    StartCoroutine(ThrowItem());

                    int coffeeClicks = coffeeFires - 1;

                    for (int i = 0; i < salarymen.Count; i++)
                    {
                        if (coffeeClicks > 0)
                        {
                            if (salarymen[i] != hit.collider.transform.parent.gameObject)
                            {
                                if (salarymen[i].GetComponent<personcontroller>().coffee.gameObject.activeInHierarchy)
                                {
                                    salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                                    coffee = Instantiate(bossCoffee, coots.transform.position, Quaternion.identity);
                                    StartCoroutine(SendItemFlying(coffee, salarymen[i].transform, false));
                                    StartCoroutine(ThrowItem());
                                    coffeeClicks--;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (personTimer <= 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                GameObject person = Instantiate(personPrefab, new Vector3(10, -3f, 0), Quaternion.identity);
                person.GetComponent<personcontroller>().postCoffeeUseSpeedScalar = postItemUseSpeedScalar;
                salarymen.Add(person);
            }
            else
            {
                GameObject person = Instantiate(personPrefab, new Vector3(-10, -3f, 0), Quaternion.identity);
                person.GetComponent<personcontroller>().postCoffeeUseSpeedScalar = postItemUseSpeedScalar;
                person.GetComponent<SpriteRenderer>().flipX = true;
                salarymen.Add(person);
            }
            personTimer = personDelay + Random.Range(-2f, 2f);

            personTimer = personTimer / Mathf.Pow(2, Mathf.FloorToInt(Time.timeSinceLevelLoad / 60f));
        }

        personTimer -= Time.deltaTime;
    }

    private IEnumerator ThrowItem()
    {
        coots.GetComponent<SpriteRenderer>().sprite = cootsThrowSprite;
        yield return new WaitForSeconds(.25f);
        coots.GetComponent<SpriteRenderer>().sprite = cootsSprite;
    }

    private IEnumerator SendItemFlying(GameObject item, Transform target, bool playAudio = true)
    {
        if (playAudio)
        {
            audioSource.PlayOneShot(catMeow);
        }
        audioSource.PlayOneShot(catThrow);

        float progress = 0;
        Vector3 startPosition = item.transform.position;
        while (progress < 1)
        {
            if (target != null)
            {
                item.transform.position = Vector3.Lerp(startPosition, target.position + new Vector3(0, curve.Evaluate(progress), 0), progress);
                progress += Time.deltaTime / .35f;
                yield return null;
            }
            else
            {
                break;
            }
        }

        item.SetActive(false);

        if (target != null)
        {
            target.GetComponent<personcontroller>().InteractWithItem(item);
            GameObject coin = Instantiate(coinPrefab, target.transform.position, Quaternion.identity);

            progress = 0;
            startPosition = coin.transform.position;
            while (progress < 1)
            {
                coin.transform.position = Vector3.Lerp(startPosition, coots.position, progress);
                progress += Time.deltaTime / Random.Range(.25f, .35f);
                yield return null;
            }
            Destroy(coin.gameObject);

            experience += 1;
            Vector3 localScale = levelUpBar.transform.localScale;
            float maxXp = experienceToLevel + experienceAdded2to20 * (level - 1) + experienceAdded20to40 * (Mathf.Max(level - 20, 0));
            localScale.x = experience / maxXp;
            audioSource.PlayOneShot(xpCollection);

            if (experience >= maxXp)
            {
                experience = 0;
                level++;
                localScale.x = 0;
                lvlText.text = "LV " + level;
                levelUpMenu.SetActive(true);

                for (int i = levelUpMenuContent.childCount - 1; i > 0; i--)
                {
                    Destroy(levelUpMenuContent.GetChild(i).gameObject);
                }

                List<GameObject> itemPrefabList = itemPrefabs.ToList();

                for (int i = 0; i < 3; i++)
                {
                    int rand = Random.Range(0, itemPrefabList.Count);
                    GameObject itemButton = Instantiate(itemPrefabList[rand], levelUpMenuContent);
                    itemButton.name = itemPrefabList[rand].name;
                    itemButton.transform.SetSiblingIndex(1);
                    itemButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Time.timeScale = 1;
                        levelUpMenu.SetActive(false);
                        SelectItem(itemButton);
                    });
                    itemPrefabList.RemoveAt(rand);
                }

                Time.timeScale = 0;
            }

            levelUpBar.transform.localScale = localScale;
        }
    }

    private void SelectItem(GameObject item)
    {
        //Debug.Log(item.name);
        switch (item.name)
        {
            case "Blunderboss":
                coffeeFires = 3;
                break;
            case "Swift":
                swift.gameObject.SetActive(true);
                break;
            case "Booster Box":
                break;
            case "Caffeine Boost":
                postItemUseSpeedScalar = 8;
                break;
            case "Carton Cannon":
                cigFires = 3;
                break;
            case "Ecig":
                break;
            case "Ders":
                ders.gameObject.SetActive(true);
                break;
            case "Level 3 Sorry":
                break;
            case "Silent Drinker":
                break;
        }
    }
}
