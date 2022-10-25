using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    LOADING,
    INTRODUCTION,
    GAME,
    END
}

[RequireComponent(typeof(SfxManager))]
public class GameManager : MonoBehaviour
{
    public GameState state { get; private set; }

    [Header("Introduction")]
    [SerializeField]
    Sprite[] numbers;
    int index = 0;

    [SerializeField]
    Image numberImage;

    [SerializeField]
    [Range(0.1f, 5f)]
    float secondsBetweenNumbers = 1f;

    [SerializeField]
    Image messageText;

    [Header("Game elements")]
    [SerializeField]
    Player player;
    [SerializeField]
    MovingBackground movingBackground;
    [SerializeField]
    MovingBackground movingMidground;
    [SerializeField]
    GameObject bottomLeft;
    [SerializeField]
    GameObject topLeft;
    [SerializeField]
    GameObject bottomRight;
    [SerializeField]
    GameObject topRight;

    [Header("Generation")]
    [SerializeField]
    [Range(25f, 75f)]
    float minHeight = 25f;

    [SerializeField]
    [Range(25f, 75f)]
    float maxHeight = 25f;

    [Header("Tube generation")]

    [SerializeField]
    GameObject tubePrefab;

    [SerializeField]
    MovingPlatform tubeContainer;

    [SerializeField]
    [Range(100f, 200f)]
    float tubeHoleHeight = 150f;

    [SerializeField]
    [Range(1f, 200f)]
    float maxHeightDifference = 50f;

    Tube lastBottomTube = null;
    float lastBottomHeight;

    [Header("Bird generation")]

    [SerializeField]
    GameObject birdPrefab;

    [SerializeField]
    GameObject birdContainer;

    [SerializeField]
    Vector2 birdGenerationTimeRange = new Vector2(5f, 10f);

    [Header("Tube speed")]

    [SerializeField]
    [Range(0f, 5f)]
    float tubeMinSpeed = 1f;

    [SerializeField]
    [Range(0f, 5f)]
    float tubeMaxSpeed = 3f;

    [SerializeField]
    [Range(0f, 1f)]
    float tubeSpeedAugmentation = 0.1f;

    bool timeToGenerateTube = false;

    [Header("Background speed")]

    [SerializeField]
    [Range(0f, 1f)]
    float backgroundMinSpeed = 0.3f;

    [SerializeField]
    [Range(0f, 2f)]
    float backgroundMaxSpeed = 15f;

    [SerializeField]
    [Range(0f, 1f)]
    float backgroundSpeedAugmentation = 0.1f;

    [Header("UI")]
    [SerializeField]
    Text scoreText;
    int score = 0;

    [Header("SFX")]
    [SerializeField]
    AudioClip winPointAudioClip;

    SfxManager sfxManager;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Screen.SetResolution(225, 400, true);
        sfxManager = GetComponent<SfxManager>();
    }

    private void Start()
    {
        state = GameState.LOADING;
        movingBackground.StartMoving(backgroundMinSpeed, backgroundMaxSpeed, backgroundSpeedAugmentation);
        movingMidground.StartMoving(backgroundMinSpeed, backgroundMaxSpeed, backgroundSpeedAugmentation);

        state = GameState.INTRODUCTION;
        StartCoroutine(IntroductionCountdown());
    }

    IEnumerator IntroductionCountdown()
    {
        yield return new WaitForSeconds(secondsBetweenNumbers);

        if (index == 0)
        {
            messageText.gameObject.SetActive(false);
            numberImage.gameObject.SetActive(true);
        }

        if (0 <= index && index < numbers.Length)
        {
            numberImage.sprite = numbers[index];
            index++;

            StartCoroutine(IntroductionCountdown());
        }
        else if (index == numbers.Length)
        {
            numberImage.sprite = null;
            numberImage.gameObject.SetActive(false);
            messageText.gameObject.SetActive(true);
            index++;
            StartCoroutine(IntroductionCountdown());
        } 
        else
        {
            numberImage.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
            StartGame();
        }
    }

    private void StartGame()
    {
        state = GameState.GAME;
        tubeContainer.StartMoving(tubeMinSpeed, tubeMaxSpeed, tubeSpeedAugmentation);
        player.StartPlaying();
        GenerateTube();
        StartCoroutine(GenerateBirdAfterTime());
    }

    private void Update()
    {
        if (state == GameState.GAME)
        {
            if (timeToGenerateTube && lastBottomTube != null && lastBottomTube.transform.position.x < player.gameObject.transform.position.x)
            {
                timeToGenerateTube = false;
                sfxManager.Play(winPointAudioClip);
                score++;
                scoreText.text = score.ToString();
                GenerateTube();
            }
        }
    }

    void GenerateTube()
    {
        float[] heights = GenerateTubesHeight();
        Vector3[] positions = GetTubesPosition(heights[0]);

        GameObject bottomTubeGo = Instantiate(tubePrefab, positions[0], Quaternion.identity);
        bottomTubeGo.transform.SetParent(tubeContainer.transform);
        Tube bottomTube = bottomTubeGo.GetComponent<Tube>();
        bottomTube.CreateTube(new Vector2(25, heights[0]), positions[0], true);

        GameObject topTubeGo = Instantiate(tubePrefab, positions[1], Quaternion.identity);
        topTubeGo.transform.SetParent(tubeContainer.transform);
        Tube topTube = topTubeGo.GetComponent<Tube>();
        topTube.CreateTube(new Vector2(25, heights[1]), positions[1], false);

        lastBottomTube = bottomTube;
        timeToGenerateTube = true;
    }

    float[] GenerateTubesHeight()
    {
        float minCurrentBottomHeight = minHeight;
        float maxCurrentBottomHeight = topRight.transform.position.y - bottomRight.transform.position.y - maxHeight - tubeHoleHeight;

        if (lastBottomTube != null)
        {
            minCurrentBottomHeight = Mathf.Max(minCurrentBottomHeight, lastBottomHeight - maxHeightDifference);
            maxCurrentBottomHeight = Mathf.Min(maxCurrentBottomHeight, lastBottomHeight + maxHeightDifference);
        }

        float bottomHeight = Random.Range(minCurrentBottomHeight, maxCurrentBottomHeight);
        float topHeight    = topRight.transform.position.y - bottomRight.transform.position.y - bottomHeight - tubeHoleHeight;

        lastBottomHeight = bottomHeight;

        return new float[] { bottomHeight, topHeight };
    }

    Vector3[] GetTubesPosition(float bottomTubeHeight)
    {
        Vector3 bottomPosition = bottomRight.transform.position;
        Vector3 topPosition = bottomPosition + Vector3.up * (bottomTubeHeight + tubeHoleHeight);

        return new Vector3[] { bottomPosition, topPosition };
    }

    IEnumerator GenerateBirdAfterTime()
    {
        float time = Random.Range(birdGenerationTimeRange[0], birdGenerationTimeRange[1]);
        yield return new WaitForSeconds(time);

        GenerateBird();
    }

    void GenerateBird()
    {
        float height = Random.Range(minHeight, maxHeight);
        Vector3 position = bottomRight.transform.position + Vector3.up * height;

        GameObject birdGO = Instantiate(birdPrefab, position, Quaternion.identity);
        birdGO.transform.SetParent(birdContainer.transform);
    }

    public void OnDefeat()
    {
        state = GameState.END;
        StopAllCoroutines();
        tubeContainer.StopMoving();
        movingBackground.StopMoving();
        movingMidground.StopMoving();
        player.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Max height for generation
        Gizmos.DrawLine(topLeft.transform.position - Vector3.up * maxHeight, topRight.transform.position - Vector3.up * maxHeight);

        // Min height for generation
        Gizmos.DrawLine(bottomLeft.transform.position + Vector3.up * minHeight, bottomRight.transform.position + Vector3.up * minHeight);

        if (lastBottomTube != null)
        {
            Gizmos.color = Color.black;

            // Max height for next generation
            Gizmos.DrawLine(new Vector3(topLeft.transform.position.x, bottomLeft.transform.position.y + lastBottomHeight + tubeHoleHeight + maxHeightDifference, topLeft.transform.position.z), new Vector3(topRight.transform.position.x, bottomLeft.transform.position.y + lastBottomHeight + tubeHoleHeight + maxHeightDifference, topRight.transform.position.z));

            // Min height for next generation
            Gizmos.DrawLine(new Vector3(bottomLeft.transform.position.x, bottomLeft.transform.position.y + lastBottomHeight - maxHeightDifference, bottomLeft.transform.position.z), new Vector3(bottomRight.transform.position.x, bottomLeft.transform.position.y + lastBottomHeight - maxHeightDifference, bottomRight.transform.position.z));
        }
    }
}
