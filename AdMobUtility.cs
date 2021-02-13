using UnityEngine;
using Cryptography = System.Security.Cryptography;
using System.Text;
#if UNITY_IOS
using UnityEngine.iOS;

#endif

/// <summary>
/// AdMobUtility
/// </summary>
public static class AdMobUtility
{
    /// <summary>
    /// Gets the test device identifier.
    /// </summary>
    /// <returns>The test device identifier.</returns>
    public static string GetTestDeviceId()
    {
#if UNITY_IOS
        return Device.advertisingIdentifier.CalculateMD5Hash();
#elif UNITY_ANDROID
		return GetAndroidId().CalculateMD5Hash().ToUpper();
#else
		return "";
#endif
    }

#if UNITY_ANDROID
	/// <summary>
	/// Gets the android identifier.
	/// </summary>
	/// <returns>The android identifier.</returns>
	public static string GetAndroidId()
	{
		var unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		var currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		var contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");  
		var secure = new AndroidJavaClass ("android.provider.Settings$Secure");
		var name = secure.GetStatic<string>("ANDROID_ID");
		var androidId = secure.CallStatic<string>("getString", contentResolver, name);
		return androidId;
	}

#endif


    #region MD5

    /// <summary>
    /// Gets the ASCII bytes.
    /// </summary>
    /// <returns>The ASCII bytes.</returns>
    /// <param name="str">String.</param>
    public static byte[] GetASCIIBytes(this string str)
    {
        var result = new byte[str.Length];
        for (var i = 0; i < str.Length; ++i)
        {
            var ch = str[i];
            result[i] = (byte) ((ch < (char) 0x80) ? ch : '?');
        }

        return result;
    }

    /// <summary>
    /// Calculates the MD5 hash.
    /// </summary>
    /// <returns>The MD5 hash.</returns>
    /// <param name="input">Input.</param>
    public static string CalculateMD5Hash(this string input) =>
        input.GetASCIIBytes().CalculateMD5Hash();

    /// <summary>
    /// Calculates the MD5 hash.
    /// </summary>
    /// <returns>The MD5 hash.</returns>
    /// <param name="input">Input.</param>
    public static string CalculateMD5Hash(this byte[] input)
    {
        var hash = Cryptography.MD5.Create().ComputeHash(input);
        var sb = new StringBuilder();
        foreach (var b in hash)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    #endregion
}