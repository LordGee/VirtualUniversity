using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public partial class Administration {

    public void Subject_Admin() {
        DeactivateActivateGroup(groupAdmin);
    }

}

public class Subject_UIGroup : MonoBehaviour {

    private Administration admin;

    public UIAdminGroups subGroupSubject;
    public UIAdminGroups subGroupAddNewCourse;
    public UIAdminGroups subGroupManageCourse;

    private UIAdminGroups current;

    private string selectedCourse;
    public string SelectedCourse {
        set { selectedCourse = value; }
        get { return selectedCourse; }
    }

    void Start () {
	    admin = FindObjectOfType<Administration>();
        current = subGroupSubject;
        current.groupObject.SetActive(true);
    }

    public void btn_Back() {
        admin.Subject_Admin();
    }

    public void btn_AddNew() {        
        DeactivateActivateGroup(subGroupAddNewCourse);
    }

    public void btn_ManageCourse() {
        selectedCourse = FindObjectOfType<SelectSubject_UIGroup>().GetSelectedCourse();
        DeactivateActivateGroup(subGroupManageCourse);
    }

    private void DeactivateActivateGroup(UIAdminGroups open) {
        current.groupObject.SetActive(false);
        current = open;
        current.groupObject.SetActive(true);
        admin.SetHeadingText(current.title);
    }
}
