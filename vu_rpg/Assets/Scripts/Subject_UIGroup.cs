﻿using System.Collections;
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
    private string selectedSubject;

    //public string SelectedCourse {
    //    set { selectedCourse = value; }
    //    get { return selectedCourse; }
    //}

    void Start () {
	    admin = FindObjectOfType<Administration>();
        current = subGroupSubject;
        current.groupObject.SetActive(true);
        FindObjectOfType<SelectSubject_UIGroup>().UpdateCourseData();
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

    public void btn_BackFromAdd() {
        DeactivateActivateGroup(subGroupSubject);
        FindObjectOfType<SelectSubject_UIGroup>().UpdateCourseData();
    }

    public void btn_AddNewCourse() {
        selectedCourse = FindObjectOfType<AddCourse_UIGroup>().GetNewCourse();
        if (selectedCourse.Length >= 3) {
            if (!Database.CheckCourseExists(selectedCourse)) {
                Database.AddNewCourse(selectedCourse);
                DeactivateActivateGroup(subGroupManageCourse);
            } else {
                string error = "The course you have entered already exists";
                FindObjectOfType<UISystemMessage>().NewTextAndDisplay(error);
                DeactivateActivateGroup(subGroupSubject);
                FindObjectOfType<SelectSubject_UIGroup>().UpdateCourseData();
            }
        } else {
            string error = "Your course name needs to be longer";
            FindObjectOfType<UISystemMessage>().NewTextAndDisplay(error);
        }
    }

    public void btn_AddNewSubject() {
        selectedSubject = FindObjectOfType<AddSubject_UIGroup>().GetNewSubject();
        string message = "";
        if (selectedCourse.Length >= 3) {
            if (!Database.CheckSubjectExists(selectedSubject)) {
                Database.AddNewSubject(selectedSubject);
                message = message + "You have successfully added  " + selectedSubject + ". ";
            } else {
                message = message + "The subject you have entered already exists.";
            }
            if (!Database.CheckSubjectLinkedToCourseExists(selectedSubject, selectedCourse)) {
                Database.AddCourseSubjects(selectedCourse, selectedSubject);
                message = message + "\nThis subject has been linked to " + selectedCourse;
            } else {
                message = message + "\nThe subject is already linked to the course.";
            }
            Message(message);
            DeactivateActivateGroup(subGroupSubject);
            FindObjectOfType<SelectSubject_UIGroup>().UpdateCourseData();
        } else {
            string error = "Your subject name needs to be longer";
            Message(error);
        }
    }

    private void DeactivateActivateGroup(UIAdminGroups open) {
        current.groupObject.SetActive(false);
        current = open;
        current.groupObject.SetActive(true);
        admin.SetHeadingText(current.title);
    }

    private void Message(string message) {
        FindObjectOfType<UISystemMessage>().NewTextAndDisplay(message);
    }
}
