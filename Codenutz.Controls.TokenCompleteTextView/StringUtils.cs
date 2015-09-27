using Android.Runtime;

namespace Codenutz.Controls
{
	public static class StringUtils
	{
		public static Java.Lang.String ToAndroidString(this string str)
		{
			return Java.Lang.Object.GetObject<Java.Lang.String>(JNIEnv.NewString(str), JniHandleOwnership.TransferLocalRef);
		}
	}
}