using UnityEngine;
using System.Runtime.CompilerServices;
using System.Diagnostics; // Conditional 사용을 위해 필요

public static class LogExtensions
{
    // UNITY_EDITOR 심볼이 있을 때만 이 함수가 실행됩니다. 
    // 즉, 안드로이드 빌드 시에는 이 함수를 호출하는 곳 자체가 사라집니다.
    [Conditional("UNITY_EDITOR")]
    [HideInCallstack]
    public static void Log(this object obj, string message, [CallerMemberName] string methodName = "")
    {
        // [클래스명 : 메서드명] 메시지 형태의 포맷
        UnityEngine.Debug.Log($"<color=#00FF00>[{obj.GetType().Name} : {methodName}]</color> {message}");
    }
}
