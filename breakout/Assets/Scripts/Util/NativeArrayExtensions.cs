using Unity.Collections;

public static class NativeArrayExtensions
{
    public static T GetRandom<T>(this NativeArray<T> array, Unity.Mathematics.Random random) where T : struct
    {
        int idx = random.NextInt(array.Length - 1);
        return array[idx];
    }
}

