using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public ScriptableObjectListSO availableTablewares;
    public TablewareInstanceListSO instantiatedTablewares;
    public IntegerSO timerTotalTime;
    public FloatSO currentTime;
    public IntegerSO score;
    public IntegerSO brokenCount;
    public IntegerSO nbCompletedTables;

    public Vector3SO playerInitPosition;
    public Vector3SO playerInitRotation;
    public Vector3SO playerCurrentPosition;
    public Vector3SO playerCurrentRotation;

    public GameObject startMenu;
    public GameObject gameOverMenu;
    public GameObject gameContainer;

    public AudioSource startGameAudio;
    public AudioSource haveMadeChoiceAudio;
    public AudioSource haveYouFinished;
    public AudioSourceSO inGameVoices;

    public ObservationSO resetObservation;

    private bool isMenu;
    private bool startGameAudioStarted;

    private void Start() {
        isMenu = true;
        startGameAudioStarted = false;
        instantiatedTablewares.SetDefaultOwner(gameContainer);
    }

    private void Update() {

        if (!isMenu) {//if we're in game
            Cursor.lockState = CursorLockMode.Locked;

            //Reset game by pressing 'R'
            if (Input.GetKeyDown(KeyCode.R)) {
                Reset();
            }

            //Checking countdown
            if (currentTime.value <= 0) {
                GameOver();
            }
        } else { //if we're in menu
            Cursor.lockState = CursorLockMode.None;
            float mixerVolume = 1;
            startGameAudio.outputAudioMixerGroup.audioMixer.GetFloat("MasterVolume", out mixerVolume);
            if (startGameAudioStarted && (!startGameAudio.isPlaying || startGameAudio.volume == 0 || mixerVolume == -80)) {
                isMenu = false;
                startMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                gameContainer.SetActive(true);
                Reset();
                startGameAudioStarted = false;
            }
        }
    }

    public void StartGame() {
        startGameAudioStarted = true;
        haveMadeChoiceAudio.Stop();
        haveYouFinished.Stop();
        startGameAudio.Play();
    }

    private void Reset() {
        //Clear HUD values
        currentTime.value = timerTotalTime.value;
        score.value = 0;

        //Clear all instantiated tablewares
        instantiatedTablewares.Clear();
        brokenCount.value = 0;

        nbCompletedTables.value = 0;

        availableTablewares.cursor = 0;
        foreach (TablewareSO t in availableTablewares.list) {
            t.ammo = t.initAmmo;
        }

        //Reset player transform
        playerCurrentPosition.value = playerInitPosition.value;
        playerCurrentRotation.value = playerInitRotation.value;

        //Reset observers
        resetObservation.Warn();
    }

    private void GameOver() {
        inGameVoices.source.Stop();
        haveYouFinished.Play();
        isMenu = true;
        gameContainer.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}
