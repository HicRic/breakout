using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Root Config")]
public class Config : ScriptableObject
{
    public PaddleCfg Paddle;

    private static Config instance;
    public static Config Instance => instance ?? (instance = Resources.Load<Config>("Config"));
}