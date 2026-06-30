namespace Miyabists2.Scripts.Relics
{
    /// <summary>
    /// 喧嚣值计数器接口。
    /// 所有角色的喧嚣值遗物都应实现此接口，
    /// 以便通用卡牌通过 MiyabiCombatService.AddDecible 正确添加喧嚣值。
    /// </summary>
    public interface IDecibleCounter
    {
        void AddCounter(int amount, bool forceAdd = false);
        void SetMax(int amount);
        void ResetMax();
        void SetThreshold(int threshold);
        void ResetThreshold();


    }
}
