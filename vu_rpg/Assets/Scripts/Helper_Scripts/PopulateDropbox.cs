using System.Collections.Generic;
using UnityEngine.UI;

public static class PopulateDropbox {

    /// <summary>
    /// Takes a given dropdown and populated the information
    /// with a string list plus adds caption
    /// </summary>
    /// <param name="dropBox">Reference to the dropdown item</param>
    /// <param name="content">A list of string containing the values to be implemented</param>
    /// <param name="caption"></param>
    public static void Run(ref Dropdown dropBox, List<string> content, string caption) {
        dropBox.GetComponent<Dropdown>().ClearOptions();
        dropBox.GetComponent<Dropdown>().AddOptions(content);
    }

}

