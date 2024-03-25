using Verse;

namespace MoreSleepAccelerators
{
    internal class Debug
    {
#if DEBUG
        public static void IsTrue(bool v1, string valueName = "Unkown Value")
        {
            if (!v1)
                Log.Error("[More Sleep Accelerators] Error Checking: " + valueName + " is false.");
        }
        public static void IsNotNull<T>(T v1, string valueName = "Unkown Value")
        {
            if (v1 == null)
                Log.Error("[More Sleep Accelerators] Error Checking: " + valueName + " is null.");
        }
        public static void AreEqual<T>(T v1, T v2, string v1Name = "v1", string v2Name = "v2")
        {
            if (!v1.Equals(v2))
                Log.Error("[More Sleep Accelerators] Error Checking: " + v1Name + " == " + v1 + " != " + v2 + " == " + v2Name);
        }
#else
        public static void IsTrue(bool _, string _2 = null) {}
        public static void IsNotNull<T>(T _, string _2 = null) {}
        public static void AreEqual<T>(T _, T _2, string _3 = null, string _4 = null) {}
#endif
    }
}
