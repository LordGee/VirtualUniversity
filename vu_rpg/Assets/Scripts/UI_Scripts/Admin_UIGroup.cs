using UnityEngine;

/// <summary>
/// Extends the administration class, however these one liners could be refactored inline
/// </summary>
public partial class Administration {

    public void Admin_Subject() {
        DeactivateActivateGroup(groupSubject);
    }

    public void Admin_Quiz() {
        DeactivateActivateGroup(groupQuiz);
    }

    public void Admin_Lecture() {
        DeactivateActivateGroup(groupLecture);
    }

    public void ExitCurrentPanel() {
        DeactivateGroup();
    }

}

/// <summary>
/// Manages the button presses for the Admin UI
/// </summary>
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

    public void btn_Lecture() {
        admin.Admin_Lecture();
    }

    public void btn_Exit() {
        admin.ExitCurrentPanel();
        admin.ExitAdministrationUI();
    }
	
}