using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Spell_System : MonoBehaviour {
    public GameObject[] slots;
    public bool[] activeSlots;
    int selectedSpell;
    string selectedSpellType;

    float inActiveValue;

    Dictionary<int, string> spells;
    
    void Start() {
        activeSlots = new bool[]{false, false, false};
        inActiveValue = 0.2f;
        selectedSpell = 666;
        spells = new Dictionary<int, string>();
        spells.Add(1, "null");
        spells.Add(2, "null");
        spells.Add(3, "null");
        setSpells("Fireball", 1);
    }

    public void setSpells(string spellType, int slot){
        int slotSelection = slot - 1;
        spells.Remove(slot);
        spells.Add(slot, spellType);
        Animator spellAnim = slots[slotSelection].GetComponent<Animator>();
        //reset animation for the slot
        resetAnimation(spellAnim);
        //set the new animation
        spellAnim.SetBool(spellType, true);
        //activate the slot
        activeSlots[slotSelection] = true;
        //set the slot to semi-transparent
        var newColor = slots[slotSelection].GetComponent<Image>().color;
        newColor.a = inActiveValue;
        slots[slotSelection].GetComponent<Image>().color = newColor;
    }

    public void selectSpell(int slot){
        int slotSelection = slot - 1;
        selectedSpell = slotSelection;

        var fadeColor = slots[0].GetComponent<Image>().color;
        fadeColor.a = inActiveValue;
        var offColor = fadeColor;
        offColor.a = 0f;

        //make all slots turn off or make semi-transparent
        if (activeSlots[0]){
            slots[0].GetComponent<Image>().color = fadeColor;
        } else {
            slots[0].GetComponent<Image>().color = offColor;
        }

        if (activeSlots[1]){
            slots[1].GetComponent<Image>().color = fadeColor;
        } else {
            slots[1].GetComponent<Image>().color = offColor;
        }

        if (activeSlots[2]){
            slots[2].GetComponent<Image>().color = fadeColor;
        } else {
            slots[2].GetComponent<Image>().color = offColor;
        }

        //turn on selected slot
        fadeColor.a = 1f;
        slots[slotSelection].GetComponent<Image>().color = fadeColor;
    }

    public void deselectSlot(int slot){
        int slotSelection = slot - 1;
        //666 = no spells are selected
        selectedSpell = 666;
        var fadeColor = slots[0].GetComponent<Image>().color;
        fadeColor.a = inActiveValue;
        slots[slotSelection].GetComponent<Image>().color = fadeColor;
    }

    private void resetAnimation(Animator anim) {
        foreach (AnimatorControllerParameter parameter in anim.parameters) {
            anim.SetBool(parameter.name, false);
        }
    }

    public int currentlySelected(){
        return selectedSpell;
    }

    public string activeSpell(){
        int current = selectedSpell + 1;
        return spells[current];
    }
}
