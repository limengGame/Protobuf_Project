using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using ServerMessage;

public class ChatModel : BaseModel<ChatModel> {
    protected override void InitAddTocHandler()
    {
        AddTocHandler(typeof(TocChat), STocChat);
    }

    private void STocChat(object data)
    {
        TocChat toc = data as TocChat;
        if (ChatView.Exists)
        {
            string content = toc.name + ":" + toc.content + ":" + toc.time;
            Debug.Log(content);
            ChatView.Instance.AddChatItem(content);
        }
    }

    public void CTosChat(string name, string content)
    {
        TosChat tos = new TosChat();
        tos.name = name;
        tos.content = content;
        tos.time = 3.3333f;
        SendTos(tos);
    }


}
