using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Administration {

    public void Admin_Subject() {
        DeactivateActivateGroup(groupSubject);
    }

    public void Admin_Quiz() {
        DeactivateActivateGroup(groupQuiz);
    }

    public void ExitCurrentPanel() {
        DeactivateGroup();
    }

}

public class Admin_UIGroup : MonoBehaviour {
    private Administration admin;

	void Start () {
	    admin = FindObjectOfType<Administration>();
	}

    public void btn_Subject() {
        admin.Admin_Subject();
    }

    public void btn_Quiz() {
        admin.Admin_Quiz();
    }

    public void btn_Exit() {
        admin.ExitCurrentPanel();
    }
	
}
