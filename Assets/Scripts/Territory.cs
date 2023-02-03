using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Territory : MonoBehaviour, IDropHandler
{

    Slider slider;
    GameObject character;
    public bool territory = false;
    public List<GameObject> cards = new List<GameObject>();
    public GameObject modal;

    public bool territoryTaken;
    private BaseTerritory baseTerritory;

    private int resourceType;
    public GameObject resourceManager;
    public float resourceTimer;
    private void Awake() {
        baseTerritory = new EasyTerritory();
        baseTerritory.Initialize();
        resourceTimer = (float) baseTerritory.ResourceRate;
        territoryTaken = false;
        resourceType = baseTerritory.ResourceType;
        // resourceManager = GameObject.FindGameObjectWithTag("RM");
    }
    public void OnDrop(PointerEventData eventdata)
    {
        character = eventdata.pointerDrag;

        character.GetComponent<RectTransform>().position = this.transform.GetChild(0).GetComponent<RectTransform>().position;
        character.GetComponent<CharacterScript>().slot = this.gameObject;
        cards.Add(character);

        if (territory == false)
        {
            character.GetComponent<CanvasGroup>().blocksRaycasts = false;
            character.GetComponent<CharacterScript>().Token.SetActive(true);
            character.GetComponent<CharacterScript>().Card.SetActive(false);

            character.GetComponent<CharacterScript>().slot = this.gameObject;
            slider = character.GetComponent<CharacterScript>().slider;
            //StartCoroutine(LoadAsynchronously());
            character.transform.SetAsFirstSibling();
            modal.GetComponent<ModalScript>().cardsSlot = cards;
            modal.GetComponent<ModalScript>().setCards();
            modal.SetActive(true);


        }

    }

    IEnumerator LoadAsynchronously()
    {
        this.GetComponent<Image>().color = Color.red;
        slider.gameObject.SetActive(true);
        slider.value = 0f;
        float timer = 0.1f;
        while (timer < 0.9f)
        {
            timer += 0.1f;
            float progress = Mathf.Clamp01(timer / .9f);
            slider.value = progress;
            yield return new WaitForSeconds(1);
        }
        slider.gameObject.SetActive(false);
        EventResult();
    }

    private void EventResult()
    {
        TakeTerritory();
    }

    private void Update() {
        if(territoryTaken){
            if(baseTerritory.ResourceTotal>0){
                resourceTimer -= Time.deltaTime;
                if(resourceTimer <= 0){
                    baseTerritory.GenerateResource();
                    Debug.Log("saudando a mandioca "+ baseTerritory.ResourceTotal);
                    resourceManager.GetComponent<ResourceManager>().AddResource(resourceType,1);
                    resourceTimer = baseTerritory.ResourceRate;
                }
            }
        }
    }
    private void TakeTerritory()
    {
        int damage = character.GetComponent<CharacterScript>().getCardAttack();
        baseTerritory.ResolveCombat(damage);
        int random = Random.Range(0, 10);
        character.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (random > 4)
        {
            this.GetComponent<Image>().color = Color.green;
            territory = true;
        }
        else
        {
            this.GetComponent<Image>().color = Color.magenta;
            territory = false;
        }
        cards.Remove(character);
        character.GetComponent<CharacterScript>().Token.SetActive(false);
        character.GetComponent<CharacterScript>().Card.SetActive(true);

        character.GetComponent<RectTransform>().position = character.GetComponent<CharacterScript>().lastPosition;

    }

    public void ModalEvent(bool choice)
    {
        modal.SetActive(false);

        if (choice)
        {
            // StartCoroutine(LoadAsynchronously());
            TakeTerritory();
            return;
        }

        territory = false;
        cards.Remove(character);
        character.GetComponent<CanvasGroup>().blocksRaycasts = true;

        character.GetComponent<CharacterScript>().Token.SetActive(false);
        character.GetComponent<CharacterScript>().Card.SetActive(true);
        character.GetComponent<RectTransform>().position = character.GetComponent<CharacterScript>().lastPosition;
    }

}
