using System.Collections.Generic;

public static class AllocationTest {
    /// <summary>
    /// Regular function used to help randomize the questions asked
    /// </summary>
    /// <param name="test">Current list of bool</param>
    /// <returns>Returns true is all items in the list have been allocated</returns>
    public static bool HasAllocationFinished(List<bool> test) {
        for (int i = 0; i < test.Count; i++) {
            if (!test[i]) { return false; }
        }
        return true;
    }
}

