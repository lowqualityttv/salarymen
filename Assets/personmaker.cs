using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioClip wrongClip = null;

    [SerializeField] private GameObject wrongSpritePrefab = null;
    [SerializeField] private Sprite[] wrongSprites = null;
    private int wrongIndex = 0;

    [SerializeField] private GameObject levelUpMenu = null;
    [SerializeField] private Transform levelUpMenuContent = null;

    [SerializeField] private List<GameObject> itemPrefabs = null;

    private List<GameObject> salarymen = new List<GameObject>();

    [SerializeField] private GameObject ders = null;
    private int dersFire = 0;

    [SerializeField] private GameObject swift = null;
    private int swiftFire = 0;

    private int coffeeFires = 1;
    private int cigFires = 1;

    private float dersTimer = 3;
    private float swiftTimer = 3;

    private float postItemUseSpeedScalar = 2;
    private bool saySorry = false;

    [SerializeField] private AudioClip sorryClip = null;
    [SerializeField] private AudioClip startClip = null;
    [SerializeField] private AudioClip mushimuClip = null;

    [SerializeField] private AudioSource louderAudioSource = null;
    [SerializeField] private GameObject startMenu = null;

    [SerializeField] private GameObject endOfGameScreen = null;
    [SerializeField] private TextMeshProUGUI levelEnd = null;
    [SerializeField] private TextMeshProUGUI coffesDelivered = null;
    [SerializeField] private TextMeshProUGUI smokesDelivered = null;
    [SerializeField] private TextMeshProUGUI accuracy = null;
    [SerializeField] private TextMeshProUGUI score = null;

    [SerializeField] private AudioClip[] ambienceClips = null;
    private int ambienceIndex = 0;
    [SerializeField] private AudioClip endOfGameClip = null;


    public bool ecigActive = false;

    [SerializeField] private ParticleSystem deathParticles = null;

    public void StartGame()
    {
        startMenu.SetActive(false);
        Time.timeScale = 1;
        louderAudioSource.PlayOneShot(mushimuClip);
    }

    private void Start()
    {
        Time.timeScale = 0;
        //Time.timeScale = 5;
    }

    public void RemoveSalaryman(GameObject man)
    {
        salarymen.Remove(man);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private float randomAmbienceTimer = 30;

    int coffeeCount = 0;
    int cigCount = 0;
    int missedCount = 0;
    int hitCount = 0;

    private void Update()
    {
        if (Time.timeSinceLevelLoad > 300)
        {
            if (!endOfGameScreen.activeInHierarchy)
            {
                for (int i = 0; i < salarymen.Count; i++)
                {
                    deathParticles.transform.position = salarymen[i].transform.position;
                    Destroy(salarymen[i]);
                    deathParticles.Emit(25);
                }
                //Time.timeScale = 0;
                levelEnd.text = "Level: " + level;
                coffesDelivered.text = "Coffees: " + coffeeCount;
                smokesDelivered.text = "Cigs: " + cigCount;
                float accuracyValue = (hitCount) / (float)(hitCount + missedCount);
                accuracy.text = "Accuracy: " + accuracyValue * 100 + "%";
                score.text = "Score: " + ((int)((coffeeCount + cigCount) * accuracyValue)).ToString();

                louderAudioSource.PlayOneShot(endOfGameClip);
                endOfGameScreen.SetActive(true);
            }
            return;
        }

        //if (isPaused)
        //{
        //    return;
        //}

        if (Time.timeScale == 0)
        {
            return;
        }

        randomAmbienceTimer -= Time.deltaTime;
        if (randomAmbienceTimer <= 0)
        {
            randomAmbienceTimer = 31;
            louderAudioSource.PlayOneShot(ambienceClips[ambienceIndex]);
            ambienceIndex++;
        }

        if (swift.activeInHierarchy)
        {
            swiftTimer -= Time.deltaTime;
            if (swiftTimer <= 0)
            {
                int swiftFireCount = swiftFire;

                for (int i = 0; i < salarymen.Count; i++)
                {
                    if (swiftFireCount > 0)
                    {
                        if (salarymen[i].GetComponent<personcontroller>().coffee.gameObject.activeInHierarchy)
                        {
                            coffeeCount++;

                            salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                            GameObject coffee = Instantiate(bossCoffee, swift.transform.position, Quaternion.identity);
                            StartCoroutine(SendItemFlying(coffee, salarymen[i].transform, false));
                            //StartCoroutine(ThrowItem());
                            swiftFireCount--;
                            swiftTimer = 3;
                        }
                    }
                    else
                    {
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
                int dersFireCount = dersFire;

                for (int i = 0; i < salarymen.Count; i++)
                {
                    if (dersFireCount > 0)
                    {
                        if (salarymen[i].GetComponent<personcontroller>().smoke.gameObject.activeInHierarchy)
                        {
                            cigCount++;

                            salarymen[i].GetComponent<personcontroller>().UpdateClicks();
                            GameObject cigs = Instantiate(cigarettes, ders.transform.position, Quaternion.identity);
                            StartCoroutine(SendItemFlying(cigs, salarymen[i].transform, false));
                            //StartCoroutine(ThrowItem());
                            dersFireCount--;
                            dersTimer = 3;
                        }
                    }
                    else
                    {
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
                    cigCount++;
                    hitCount++;

                    int cigClicks = cigFires - 1;

                    for (int i = 0; i < salarymen.Count; i++)
                    {
                        if (cigClicks > 0)
                        {
                            if (salarymen[i] != hit.collider.transform.parent.gameObject)
                            {
                                if (salarymen[i].GetComponent<personcontroller>().smoke.gameObject.activeInHierarchy)
                                {
                                    cigCount++;
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

            if (hit.collider != null && hit.collider.tag == "Coffee")
            {
                missedCount++;

                if (saySorry)
                {
                    louderAudioSource.PlayOneShot(sorryClip, 1);
                }
                else
                {
                    louderAudioSource.PlayOneShot(wrongClip, 1);
                    GameObject wrongSprite = Instantiate(wrongSpritePrefab, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), Quaternion.identity);
                    wrongSprite.GetComponent<SpriteRenderer>().sprite = wrongSprites[wrongIndex];
                    wrongIndex++;
                    if (wrongIndex >= wrongSprites.Length)
                    {
                        wrongIndex = 0;
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

                    coffeeCount++;
                    hitCount++;

                    int coffeeClicks = coffeeFires - 1;

                    for (int i = 0; i < salarymen.Count; i++)
                    {
                        if (coffeeClicks > 0)
                        {
                            if (salarymen[i] != hit.collider.transform.parent.gameObject)
                            {
                                if (salarymen[i].GetComponent<personcontroller>().coffee.gameObject.activeInHierarchy)
                                {
                                    coffeeCount++;

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

            if (hit.collider != null && hit.collider.tag == "Cigs")
            {
                missedCount++;

                if (saySorry)
                {
                    louderAudioSource.PlayOneShot(sorryClip, 1);
                }
                else
                {
                    louderAudioSource.PlayOneShot(wrongClip, 1);
                    GameObject wrongSprite = Instantiate(wrongSpritePrefab, new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), Quaternion.identity);
                    wrongSprite.GetComponent<SpriteRenderer>().sprite = wrongSprites[wrongIndex];
                    wrongIndex++;
                    if (wrongIndex >= wrongSprites.Length)
                    {
                        wrongIndex = 0;
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

            personTimer = personTimer / Mathf.Pow(2.5f, Mathf.FloorToInt(Time.timeSinceLevelLoad / 60f));
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

                for (int i = levelUpMenuContent.childCount - 2; i > 0; i--)
                {
                    Destroy(levelUpMenuContent.GetChild(i).gameObject);
                }

                List<GameObject> itemPrefabList = itemPrefabs.ToList();

                for (int i = 0; i < 3; i++)
                {
                    int rand = Random.Range(0, itemPrefabList.Count);
                    GameObject prefab = itemPrefabList[rand];
                    GameObject itemButton = Instantiate(itemPrefabList[rand], levelUpMenuContent);
                    itemButton.name = itemPrefabList[rand].name;
                    itemButton.transform.SetSiblingIndex(1);
                    itemButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        for (int j = levelUpMenuContent.childCount - 2; j > 0; j--)
                        {
                            levelUpMenuContent.GetChild(j).GetComponent<Image>().color = Color.white;
                        }

                        itemButton.GetComponent<Image>().color = Color.green;
                        SetSelectedItem(prefab);
                    });
                    itemPrefabList.RemoveAt(rand);
                }

                Time.timeScale = 0;
            }

            levelUpBar.transform.localScale = localScale;
        }
    }

    private GameObject selectedItemPrefab = null;

    private void SetSelectedItem(GameObject item)
    {
        selectedItemPrefab = item;
    }

    public void SelectItem()
    {
        if (selectedItemPrefab == null)
        {
            return;
        }

        switch (selectedItemPrefab.name)
        {
            case "Blunderboss":
                coffeeFires++;
                break;
            case "Swift":
                swift.gameObject.SetActive(true);
                swiftFire++;
                break;
            case "Booster Box":
                break;
            case "Caffeine Boost":
                postItemUseSpeedScalar = 8;
                itemPrefabs.Remove(selectedItemPrefab);
                break;
            case "Carton Cannon":
                cigFires++;
                break;
            case "Ecig":
                ecigActive = true;
                itemPrefabs.Remove(selectedItemPrefab);
                break;
            case "Ders":
                ders.gameObject.SetActive(true);
                dersFire++;
                break;
            case "Level 3 Sorry":
                saySorry = true;
                itemPrefabs.Remove(selectedItemPrefab);
                break;
            case "Silent Drinker":
                break;
        }

        Time.timeScale = 1;
        levelUpMenu.SetActive(false);
        selectedItemPrefab = null;
    }
}
