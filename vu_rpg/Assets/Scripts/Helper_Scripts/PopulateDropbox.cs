using System.Collections.Generic;
using UnityEngine.UI;

public static class PopulateDropbox {

    public static void Run(ref Dropdown dropBox, List<string> content, string caption) {
        dropBox.GetComponent<Dropdown>().ClearOptions();
        dropBox.GetComponent<Dropdown>().AddOptions(content);
    }

}

