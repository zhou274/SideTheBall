#define NO_FIREBASE
#if !UNITY_WEBGL && !NO_FIREBASE
using Firebase.Analytics;
#endif

public class CFirebase {

	public static void LogEvent(string contentType, string itemID, double value)
    {
#if !UNITY_WEBGL && !NO_FIREBASE
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectContent,
            new Parameter(FirebaseAnalytics.ParameterContentType, contentType),
            new Parameter(FirebaseAnalytics.ParameterItemId, itemID),
            new Parameter(FirebaseAnalytics.ParameterValue, value));
#endif
    }

    public static void LogEvent(string contentType, string itemID)
    {
#if !UNITY_WEBGL && !NO_FIREBASE
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectContent,
            new Parameter(FirebaseAnalytics.ParameterContentType, contentType),
            new Parameter(FirebaseAnalytics.ParameterItemId, itemID));
#endif
    }

    public static void LogEvent(string contentType)
    {
#if !UNITY_WEBGL && !NO_FIREBASE
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectContent,
            new Parameter(FirebaseAnalytics.ParameterContentType, contentType));
#endif
    }
}
