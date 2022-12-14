using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    public GameObject brokenPrefab;
    public TablewareInstanceListSO instantiatedTablewares;
    public IntegerSO brokenCount;
    public AudioSourceSO inGameVoices;
    public AudioClip[] breakVoiceClips;
    public AudioSourceSO breakDishesAudio;
    public AudioClip[] breakDishesClips;

    private bool alreadyCollided;

    private void Start() {
        alreadyCollided = false;
    }
    private void OnCollisionEnter(Collision collision) {
        bool isBreaker = collision.gameObject.tag == "Tableware" || collision.gameObject.tag == "Breaker";
        if (isBreaker && !alreadyCollided) {
            GameObject brokenClone = Instantiate<GameObject>(brokenPrefab, transform.position, transform.rotation);
            //Raise the broken object a little bit, otherwise it comes up with an explosion (can't really understand why...)
            brokenClone.transform.position = new Vector3(brokenClone.transform.position.x, brokenClone.transform.position.y + 0.5f, brokenClone.transform.position.z);
            brokenClone.transform.parent = transform.parent;
            
            instantiatedTablewares.Remove(gameObject);
            TablewareInstanceListSO.Container container = new TablewareInstanceListSO.Container();
            container.instance = brokenClone;
            instantiatedTablewares.list.Add(container);

            brokenCount.value++;
            if (!inGameVoices.source.isPlaying) {
                inGameVoices.source.clip = breakVoiceClips[Random.Range(0, breakVoiceClips.Length)];
                inGameVoices.source.pitch = Random.Range(0.95f, 1.1f);
                inGameVoices.source.Play();
            }
            if (!breakDishesAudio.source.isPlaying) {
                breakDishesAudio.source.clip = breakDishesClips[Random.Range(0, breakDishesClips.Length)];
                breakDishesAudio.source.pitch = Random.Range(0.8f, 1.2f);
                breakDishesAudio.source.Play();
            }
            Destroy(gameObject);
            alreadyCollided = true;
        }
    }
}
