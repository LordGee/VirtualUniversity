using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class AllocationTest {
    public static bool HasAllocationFinished(List<bool> test) {
        for (int i = 0; i < test.Count; i++) {
            if (!test[i]) { return false; }
        }
        return true;
    }
}

