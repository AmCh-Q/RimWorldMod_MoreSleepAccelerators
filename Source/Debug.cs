using Verse;

namespace MoreSleepAccelerators;

internal static class Debug
{
	//[Conditional("DEBUG")]
	public static void Warn(string str)
		=> Log.Warning(str);

	//[Conditional("DEBUG")]
	public static void IsTrue(bool v1, string valueName = "Unkown Value")
	{
		if (!v1)
			Log.Error("[More Sleep Accelerators] Error Checking: " + valueName + " is false.");
	}

	//[Conditional("DEBUG")]
	public static void IsNotNull<T>(T v1, string valueName = "Unkown Value")
	{
		if (v1 == null)
			Log.Error("[More Sleep Accelerators] Error Checking: " + valueName + " is null.");
	}

	//[Conditional("DEBUG")]
	public static void AreEqual<T>(T v1, T v2, string v1Name = "v1", string v2Name = "v2")
	{
		if (!v1!.Equals(v2))
			Log.Error("[More Sleep Accelerators] Error Checking: " + v1Name + " == " + v1 + " != " + v2 + " == " + v2Name);
	}
}
