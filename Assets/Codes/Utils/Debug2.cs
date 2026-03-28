using UnityEngine;
using System.Runtime.CompilerServices;

public static class Debug2
{
    [HideInCallstack]
    public static void Log(this object obj, string message, [CallerMemberName] string methodName = "")
    {
        // [클래스명 : 메서드명] 메시지 형태의 포맷
        Debug.Log($"<color=#00FF00>[{obj.GetType().Name} : {methodName}]</color> {message}");
    }
}
