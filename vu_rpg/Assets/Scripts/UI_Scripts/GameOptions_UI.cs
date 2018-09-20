
public partial class UIChat {
    private bool toggle;

    void Start() {
        toggle = panel.activeSelf;
    }

    public void ToggleChat() {
        toggle = !toggle;
        panel.SetActive(toggle);
    }
}

public partial class UIMinimap {
    private bool toggle;

    void InitStart() {
        toggle = panel.activeSelf;
    }

    public void ToggleMap() {
        toggle = !toggle;
        panel.SetActive(toggle);
    }
}
