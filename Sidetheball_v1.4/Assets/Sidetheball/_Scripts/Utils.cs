using UnityEngine;

namespace dotmob
{
    public class Utils
    {
        public static float lastMusicTime;
        public static int lastMusicIndex;

        public static Level GetLevel( int worldIndex, int levelIndex)
        {
            return Resources.Load<Level>("World_" + worldIndex + "/Level_" + levelIndex);
        }

        public static void SetMusic()
        {
            if (lastMusicIndex == 0)
            {
                lastMusicIndex = CUtils.GetRandom(1, 2);
                Music.instance.Play((Music.Type)lastMusicIndex);
                lastMusicTime = Time.time;
            }
            else if (Time.time - lastMusicTime > 30)
            {
                lastMusicIndex = 3 - lastMusicIndex;
                Music.instance.Play((Music.Type)lastMusicIndex);
                lastMusicTime = Time.time;
            }
        }
    }
}