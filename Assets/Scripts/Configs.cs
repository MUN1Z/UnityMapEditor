namespace Assets.Scripts
{
    public class Configs
    {
        public static bool EnableObjectOcclusion => true;

        public static float OcclusionSampleDelay => 1f;
        public static float OcclusionHideDelay => 1f;

        public static float ViewDistance => 100000000000f;
        public static int OcclusionSamples => 60;
        public static float MinOcclusionDistance => 1f;
    }
}
