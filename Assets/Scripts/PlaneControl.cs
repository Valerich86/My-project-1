using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlaneControl : MonoBehaviour
{
    [Header("\t\t\t\t\t\tSettings:")]
    public BallControl Ball;
    public ExplosionControl Explosion;
    public GameObject CoinPrefab;
    public GameObject HourglassPrefab;
    public GameObject TorpedoPrefab;
    public GameObject HeartPrefab;
    public GameObject HeartExplosionPrefab;
    public GameObject ScorePrefab;
    public GameObject JawsPrefab;
    public GameObject TimerImage;
    public Image HPCounter;
    public TextMeshProUGUI CoinsCounter;
    public TextMeshProUGUI BonusMessage;
    public TextMeshProUGUI TimeCounter;
    public TextMeshProUGUI ScoreCounter;
    public TextMeshProUGUI TimeCounter2;
    public TextMeshProUGUI ScoreCounter2;
    public TextMeshProUGUI Level;
    public Canvas LevelUpScreen;
    public AudioClip Count;
    public AudioClip Bonus;
    public AudioClip Luck;
    public AudioClip Failure;
    public AudioClip FallDown;
    public AudioClip Emerging;
    public AudioClip Torpedo;
    public AudioClip Click;
    public AudioClip Splash;
    public AudioClip Growl;
    public AudioClip Damage;
    public AudioClip ScoresFallDown;
    
    [SerializeField] private int _coins;
    [SerializeField] private float _seconds;

    [HideInInspector] public List<GameObject> CoinsOnLevel;
    [HideInInspector] public bool StopGame;

    public event Action BallFell;
    public float HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = GameManager.Instance.HP;
            if (_hp > 1) _hp = 1;
        }
    }

    private float _hp;
    private Quaternion _startRotation;
    private AudioSource _source;
    private Animator _animator;
    private GameObject _heart;
    private Transform _coinSpawnPosition;
    private Transform _hourglassSpawnPosition;
    private Transform _torpedoSpawnPosition;
    private Transform _heartSpawnPosition;
    private Transform _jawsSpawnPosition;
    private Transform _scoresSpawnPosition;
    private int _coinsNeeded;
    private float _rotationSpeed;
    private float _hourGlassTimeToRespawn;
    private float _torpedoTimeToRespawn;
    private int _score;
    private int _torpedoQuantity;
    private int _level;
    private bool _levelUpIsCalled;
    private bool _hourglassIsActive;
    private bool _torpedoIsActive;

    void Start()
    {
        _coinSpawnPosition = transform.Find("CoinSpawn");
        _hourglassSpawnPosition = transform.Find("HourglassSpawn");
        _torpedoSpawnPosition = transform.Find("TorpedoSpawn");
        _heartSpawnPosition = transform.Find("HeartSpawn");
        _jawsSpawnPosition = transform.Find("JawsSpawn");
        _scoresSpawnPosition = transform.Find("ScoresSpawn");   
        Ball.TorpedoAttack += TakeDamage;
        Ball.CoinPickedUp += CoinRespawn;
        Ball.HourglassPickedUp += HourglassPicking;
        Ball.HeartPickedUp += HeartPicking;
        Ball.ScorePickedUp += ScorePicking;
        BonusMessage.gameObject.SetActive(false);
        LevelUpScreen.gameObject.SetActive(false);
        _source = GetComponent<AudioSource>();
        _source.PlayOneShot(FallDown);
        _animator = TimerImage.GetComponent<Animator>();
        _startRotation = transform.rotation;
        _coinsNeeded = GameManager.Instance.CoinsNeeded;
        _score = GameManager.Instance.Score;
        _hp = GameManager.Instance.HP;
        _level = GameManager.Instance.Level;
        _torpedoQuantity = GameManager.Instance.TorpedoQuantity;
        _rotationSpeed = GameManager.Instance.RotationSpeed;
        StopGame = false;
        _levelUpIsCalled = false;
        CoinsOnLevel = new List<GameObject>();
        CoinRespawn(0);
        StartCoroutine(TorpedoRespawn());
        StartCoroutine(HourGlassRespawn());
        StartCoroutine(HeartRespawn());
        StartCoroutine(JawsRespawn());
        StartCoroutine(ScoreRespawn());
    }

    

    void Update()
    {
        if (_hp == 0 || (_seconds <= 0 && _coins < _coinsNeeded))
        {
            StopGame = true;
            Invoke(nameof(CallGameOver), 5f);
            _source.PlayOneShot(Failure);
        }
        
        if (StopGame == false)
        {
            if (Input.GetKey(KeyCode.D))
                transform.rotation *= Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, new Vector3(0, 1, 0));
            else if (Input.GetKey(KeyCode.A))
                transform.rotation *= Quaternion.AngleAxis(-_rotationSpeed * Time.deltaTime, new Vector3(0, 1, 0));
            else if (Input.GetKey(KeyCode.W))
                transform.rotation *= Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, new Vector3(1, 0, 0));
            else if (Input.GetKey(KeyCode.S))
                transform.rotation *= Quaternion.AngleAxis(-_rotationSpeed * Time.deltaTime, new Vector3(1, 0, 0));
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, _startRotation, Time.deltaTime);
        }

        HPCounter.GetComponent<Image>().fillAmount = _hp;
        ScoreCounter.text = $"{_score} очков";
        CoinsCounter.text = $"{_coins}/{_coinsNeeded}";
        Level.text = $"уровень № {_level}";
    }

    private void FixedUpdate()
    {
        if(StopGame == false)
        {
            if (Mathf.Round(_seconds) == 10)
            {
                _animator.SetTrigger("TimerSignal");
                _source.PlayOneShot(Count);
            } 
            _seconds -= Time.deltaTime;
        }

        TimeCounter.text = (_seconds >= 0) ? Mathf.Round(_seconds).ToString() : "0";
        ScoreCounter2.text = ScoreCounter.text;
        TimeCounter2.text = TimeCounter.text;
    }

    private void AddLife()
    {
        _source.PlayOneShot(Bonus);
        _hp += .1f;
        BonusMessage.gameObject.SetActive(true);
        Instantiate(Ball.CoinEffect, BonusMessage.transform.position, BonusMessage.transform.rotation);
        BonusMessage.text = "+10 к здоровью!";
        Invoke(nameof(HideMessage), 3f);
    }

    private void HideMessage() => BonusMessage.gameObject.SetActive(false);
  


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && StopGame == false)
        {
            _source.PlayOneShot(FallDown);
            _hp -= .1f;
            if (_hp != 0) BallFell?.Invoke();
        }
        if (other.CompareTag("Coin") && StopGame == false)
        {
            Destroy(other.gameObject);
            CoinRespawn(0);
        }
    }


    private void CoinRespawn(int bonus)
    {
        _coins += bonus;
        _score += bonus*10;
        if (_score % 200 == 0 && _score != 0) AddLife();
        if (_coins >= _coinsNeeded)
        {
            StopGame = true;
            _source.PlayOneShot(Luck);
            StartCoroutine("ScoreCount");
            
        }
        else if (StopGame == false)
        {
            _source.PlayOneShot(Emerging);
            _coinSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-9, 9), 0, UnityEngine.Random.Range(-9, 9));
            GameObject newCoin = Instantiate(CoinPrefab, _coinSpawnPosition);
            CoinsOnLevel.Add(newCoin);
            if (CoinsOnLevel.Count > 1)
            {
                foreach (var coin in CoinsOnLevel)
                {
                    Destroy(coin);
                }
                CoinsOnLevel.Clear();
                CoinRespawn(0);
            }
        }
    }

    private IEnumerator HourGlassRespawn()
    {
        if(StopGame == false)
        {
            int spawnTime = UnityEngine.Random.Range(20, 50);
            yield return new WaitForSeconds(spawnTime);
            _source.PlayOneShot(Emerging);
            _hourglassSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-7, 7), 0, UnityEngine.Random.Range(-7, 7));
            Instantiate(HourglassPrefab, _hourglassSpawnPosition);
            StartCoroutine(HourGlassRespawn());
        }
    }

    private IEnumerator HeartRespawn()
    {
        if (StopGame == false && _level > 3)
        {
            int spawnTime = UnityEngine.Random.Range(0, 90);
            yield return new WaitForSeconds(spawnTime);
            _source.PlayOneShot(Emerging);
            _heartSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-7, 7), 0, UnityEngine.Random.Range(-7, 7));
            _heart = Instantiate(HeartPrefab, _heartSpawnPosition);
            if (_heart != null) Invoke(nameof(HeartExplose), 10f);
        }
    }


    private IEnumerator JawsRespawn()
    {
        if (StopGame == false)
        {
            int spawnTime = UnityEngine.Random.Range(20, 50);
            yield return new WaitForSeconds(spawnTime);
            _source.PlayOneShot(Growl);
            _jawsSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-3, 3), 20, UnityEngine.Random.Range(-3, 3));
            Instantiate(JawsPrefab, _jawsSpawnPosition);
        }
    }

    
    private IEnumerator TorpedoRespawn()
    {
        if(StopGame == false)
        {
            int spawnTime = UnityEngine.Random.Range(20, 50);
            yield return new WaitForSeconds(spawnTime);
            for (int i = 0; i < _torpedoQuantity; i++)
            {
                if (StopGame == false)
                {
                    _source.PlayOneShot(Torpedo);
                    _torpedoSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-7, 7), 20, UnityEngine.Random.Range(-7, 7));
                    yield return new WaitForSeconds(0.5f);
                    Instantiate(TorpedoPrefab, _torpedoSpawnPosition);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            StartCoroutine(TorpedoRespawn());
        }
    }


    private IEnumerator ScoreRespawn()
    {
        if (StopGame == false)
        {
            int spawnTime = UnityEngine.Random.Range(20, 90);
            yield return new WaitForSeconds(spawnTime);
            _source.PlayOneShot(ScoresFallDown);
            for (int i = 0; i < 50; i++)
            {
                _scoresSpawnPosition.position = new Vector3(UnityEngine.Random.Range(-9, 9), 0, UnityEngine.Random.Range(-9, 9));
                Instantiate(ScorePrefab, _scoresSpawnPosition);
            }
        }
    }

    private void HourglassPicking()
    {
        float bonus = 10f;
        if (_level > 5) bonus = UnityEngine.Random.Range(10, 30);
        if (_level > 10) bonus = UnityEngine.Random.Range(20, 60);
        _seconds += bonus;
        BonusMessage.gameObject.SetActive(true);
        Instantiate(Ball.CoinEffect, BonusMessage.transform.position, BonusMessage.transform.rotation);
        BonusMessage.text = $"+{bonus} секунд!";
        Invoke(nameof(HideMessage), 3f);
    }


    private void ScorePicking() => _score += 10;

    private void HeartPicking()
    {
        if(StopGame == false)
        {
            _hp = 1f;
            BonusMessage.gameObject.SetActive(true);
            Instantiate(Ball.CoinEffect, BonusMessage.transform.position, BonusMessage.transform.rotation);
            BonusMessage.text = $"Здоровье пополнено!";
            Invoke(nameof(HideMessage), 3f);
        }
    }

    private IEnumerator ScoreCount()
    {
        
        yield return new WaitForSeconds(2f);
        LevelUpScreen.gameObject.SetActive(true);
        while (_seconds > 0)
        {
            _source.PlayOneShot(Click);
            _seconds -= 1;
            _score += 100;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2f);
        if (_levelUpIsCalled == false)
        {
            _levelUpIsCalled = true;
            LevelUpScreen.gameObject.SetActive(false);
            GameManager.Instance.LevelUp(_score, _hp);
        }
    }


    public void TakeDamage()
    {
        _source.PlayOneShot(Damage);
        _hp -= .01f;
        Debug.Log(_hp);
    }

    private void CallGameOver() => GameManager.Instance.GameOverScene();

    private void HeartExplose()
    {
        _source.PlayOneShot(Splash);
        Instantiate(HeartExplosionPrefab, _heart.transform.position, _heart.transform.rotation);
        Destroy(_heart);
    }
}
