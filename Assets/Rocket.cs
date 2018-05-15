using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rotThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
 

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] int numLife =3;

    Rigidbody rigidBody;
    AudioSource audioSource;

    public static int Score;


    public GameObject mainMenuCanvas;
    public GameObject deathMenuCanvas;
    public GameObject howToPlayCanvas;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    bool collisionsDisabled = false;

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            state = State.Transcending;
        }
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        if (Input.GetKeyDown(KeyCode.R) && state == State.Dying)
        {
            SceneManager.LoadScene(0);
        }
        else if(Input.GetKeyDown(KeyCode.H) && state == State.Dying)
        {
            LoadFirstLevel();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        deathParticles.Play();
        audioSource.PlayOneShot(death);
        Invoke("DeathMenu", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        if(SceneManager.GetActiveScene().buildIndex == 19)
        {
            Invoke("LoadFirstLevel", levelLoadDelay);
        }
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void DeathMenu()
    {
        deathMenuCanvas.SetActive(!deathMenuCanvas.activeSelf);
    }

    private void LoadFirstLevel()
    {
        Score = SceneManager.GetActiveScene().buildIndex +1;
        if (PlayerPrefs.HasKey("CurrentHighScore"))
        {
            PlayerPrefs.DeleteKey("CurrentHighScore");
        }
        PlayerPrefs.SetInt("CurrentHighScore", Score);
        SceneManager.LoadScene(20);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        /* if (nextSceneIndex == 20)
         {
             nextSceneIndex = 21; // change to end screen
         }*/
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
      
        float rotationThisFrame = rotThrust * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
    public void Play()
    {
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        state = State.Alive;
    }

    public void HowToPlay()
    {
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        howToPlayCanvas.SetActive(!howToPlayCanvas.activeSelf);
    }


    public void Retry()
    {       
        SceneManager.LoadScene(0);
    }
    public void SetHighScore()
    {
        LoadFirstLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void HighScoresButton()
    {
        SceneManager.LoadScene(20);
    }



}
 