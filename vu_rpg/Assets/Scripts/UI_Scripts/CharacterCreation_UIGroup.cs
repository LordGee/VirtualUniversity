using System;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class UICharacterCreation {

    public Button nextButton;

    public void NextButtonAction() {
        if (nameInput.text.Trim() == "") {
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay("Please enter a valid name");
        } else {
            nextButton.gameObject.SetActive(false);
            createButton.gameObject.SetActive(true);
            nameInput.gameObject.SetActive(false);
            classDropdown.gameObject.SetActive(true);
        }
    }

}
