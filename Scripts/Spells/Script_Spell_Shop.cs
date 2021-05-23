using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Spell_Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _NotifRef = GameObject.FindGameObjectWithTag("Notification").GetComponent<CS_Notifications>();

        _Delay = false;
        _StoreStatus = false;

        Button ss1 = SpellSlot1.GetComponent<Button>();
        ss1.onClick.AddListener(TaskOnClickS1);
        Button ss2 = SpellSlot2.GetComponent<Button>();
        ss2.onClick.AddListener(TaskOnClickS2);
        Button ss3 = SpellSlot3.GetComponent<Button>();
        ss3.onClick.AddListener(TaskOnClickS3);
        Button back = Back.GetComponent<Button>();
        back.onClick.AddListener(TaskOnClickB);

        SpellSlot1.SetActive(false);
        SpellSlot2.SetActive(false);
        SpellSlot3.SetActive(false);
        Back.SetActive(false);

        iceButton1.SetActive(false);
        fireButton1.SetActive(false);
        waterButton1.SetActive(false);
        iceButton2.SetActive(false);
        fireButton2.SetActive(false);
        waterButton2.SetActive(false);
        iceButton3.SetActive(false);
        fireButton3.SetActive(false);
        waterButton3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (Input.GetKey(KeyCode.E) && !_StoreStatus && !_Delay) {
                _StoreStatus = true; // whether the store is currently active
                _Delay = true; // cut the if logic
                StartCoroutine("StoreKeyDelay"); // start 0.5 second key-press delay 
                other.GetComponent<Script_Player_Movement>().SetAttackStatus(true); //prevent player from pressing lmb or rmb
                LoadStore(); // Show UI components
            }
            if (Input.GetKey(KeyCode.E) && _StoreStatus && !_Delay) {
                _StoreStatus = false; // whether the store is currently active
                _Delay = true; // cut the if logic
                StartCoroutine("StoreKeyDelay"); // start 0.5 second key-press delay 
                other.GetComponent<Script_Player_Movement>().SetAttackStatus(false); // allow the player to use lmb or rmd again
                UnloadStore(); // remove UI components
            }
        }
    }

    //when the player enters the shop, activate the option buttons
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _NotifRef.SetActiveNotif(true);
            _NotifRef.SetText("Swap Spells");
        }
    }

    //when the player leaves, deactivate all buttons
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _NotifRef.SetActiveNotif(false);
            other.GetComponent<Script_Player_Movement>().SetAttackStatus(false); // allow the player to use lmb or rmd again
            UnloadStore();
        }
    }

    void LoadStore() {
        SpellSlot1.SetActive(true);
        SpellSlot2.SetActive(true);
        SpellSlot3.SetActive(true);
    }

    void UnloadStore() {
        SpellSlot1.SetActive(false);
        SpellSlot2.SetActive(false);
        SpellSlot3.SetActive(false);
        Back.SetActive(false);

        iceButton1.SetActive(false);
        fireButton1.SetActive(false);
        waterButton1.SetActive(false);
        iceButton2.SetActive(false);
        fireButton2.SetActive(false);
        waterButton2.SetActive(false);
        iceButton3.SetActive(false);
        fireButton3.SetActive(false);
        waterButton3.SetActive(false);
    }

    IEnumerator StoreKeyDelay() {
        yield return new WaitForSeconds(0.5f);
        _Delay = false;
        yield return 0;
    }

    //on click, deactiate the option buttons, activate the back button, and activate the corresponding spell buttons
    void TaskOnClickS1()
    {
        SpellSlot1.SetActive(false);
        SpellSlot2.SetActive(false);
        SpellSlot3.SetActive(false);
        Back.SetActive(true);

        iceButton1.SetActive(true);
        fireButton1.SetActive(true);
        waterButton1.SetActive(true);
    }
    //on click, deactiate the option buttons, activate the back button, and activate the corresponding spell buttons
    void TaskOnClickS2()
    {
        SpellSlot1.SetActive(false);
        SpellSlot2.SetActive(false);
        SpellSlot3.SetActive(false);
        Back.SetActive(true);

        iceButton2.SetActive(true);
        fireButton2.SetActive(true);
        waterButton2.SetActive(true);
    }
    //on click, deactiate the option buttons, activate the back button, and activate the corresponding spell buttons
    void TaskOnClickS3()
    {
        SpellSlot1.SetActive(false);
        SpellSlot2.SetActive(false);
        SpellSlot3.SetActive(false);
        Back.SetActive(true);

        iceButton3.SetActive(true);
        fireButton3.SetActive(true);
        waterButton3.SetActive(true);
    }

    //Oposite of the option buttons
    void TaskOnClickB()
    {
        SpellSlot1.SetActive(true);
        SpellSlot2.SetActive(true);
        SpellSlot3.SetActive(true);
        Back.SetActive(false);

        iceButton1.SetActive(false);
        fireButton1.SetActive(false);
        waterButton1.SetActive(false);
        iceButton2.SetActive(false);
        fireButton2.SetActive(false);
        waterButton2.SetActive(false);
        iceButton3.SetActive(false);
        fireButton3.SetActive(false);
        waterButton3.SetActive(false);
    }

    //three main menu buttons
    public GameObject SpellSlot1;
    public GameObject SpellSlot2;
    public GameObject SpellSlot3;
    public GameObject Back;


    
    public GameObject fireButton1;
    public GameObject fireButton2;
    public GameObject fireButton3;

    public GameObject iceButton1;
    public GameObject iceButton2;
    public GameObject iceButton3;

    public GameObject waterButton1;
    public GameObject waterButton2;
    public GameObject waterButton3;

    private CS_Notifications _NotifRef;
    private bool _StoreStatus;
    private bool _Delay;
}
