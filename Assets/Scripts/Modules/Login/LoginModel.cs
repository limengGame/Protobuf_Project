using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerMessage;

public class LoginModel : BaseModel<LoginModel>
{
    protected override void InitAddTocHandler()
    {
        AddTocHandler(typeof(SignUpResponse), STocLogin);
    }

    private void STocLogin(object data)
    {
        SignUpResponse res = data as SignUpResponse;
        Debug.Log("STocLogin : " + res.version);
    }

}
