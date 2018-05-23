﻿using UnityEngine;
using UnityEngine.UI;

public partial class UIChat : MonoBehaviour {
    public GameObject panel;
    public InputField messageInput;
    public Button sendButton;
    public Transform content;
    public ScrollRect scrollRect;
    public GameObject textPrefab;
    public KeyCode[] activationKeys = {KeyCode.Return, KeyCode.KeypadEnter};
    public int keepHistory = 100; // only keep 'n' messages

    void Update() {
        Player player = Utils.ClientLocalPlayer();
        panel.SetActive(player != null); // hide while not in the game world
        if (!player) return;

        // character limit
        Chat chat = player.GetComponent<Chat>();
        messageInput.characterLimit = chat.maxLength;

        // activation
        if (Utils.AnyKeyUp(activationKeys)) messageInput.Select();

        // end edit listener
        messageInput.onEndEdit.SetListener((value) => {
            // submit key pressed? then submit and set new input text
            if (Utils.AnyKeyDown(activationKeys)) {
                string newinput = chat.OnSubmit(value);
                messageInput.text = newinput;
                messageInput.MoveTextEnd(false);
            }

            // unfocus the whole chat in any case. otherwise we would scroll or
            // activate the chat window when doing wsad movement afterwards
            UIUtils.DeselectCarefully();
        });

        // send button
        sendButton.onClick.SetListener(() => {
            // submit and set new input text
            string newinput = chat.OnSubmit(messageInput.text);
            messageInput.text = newinput;
            messageInput.MoveTextEnd(false);

            // unfocus the whole chat in any case. otherwise we would scroll or
            // activate the chat window when doing wsad movement afterwards
            UIUtils.DeselectCarefully();
        });

        // addon system hooks
        Utils.InvokeMany(typeof(UIChat), this, "Update_");
    }

    void AutoScroll() {
        // update first so we don't ignore recently added messages, then scroll
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void AddMessage(MessageInfo msg) {
        // delete old messages so the UI doesn't eat too much performance.
        // => every Destroy call causes a lag because of a UI rebuild
        // => it's best to destroy a lot of messages at once so we don't
        //    experience that lag after every new chat message
        if (content.childCount >= keepHistory) {
            for (int i = 0; i < content.childCount / 2; ++i)
                Destroy(content.GetChild(i).gameObject);
        }

        // instantiate and initialize text prefab
        GameObject go = Instantiate(textPrefab);
        go.transform.SetParent(content.transform, false);
        go.GetComponent<Text>().text = msg.content;
        go.GetComponent<Text>().color = msg.color;

        AutoScroll();
    }
}
