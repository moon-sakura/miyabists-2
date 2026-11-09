using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace Miyabists2.Scripts._Guang.Powers
{
    /// <summary>
    /// 进入明心镜状态时的额外结算。
    /// 叶瞬光进入明心镜会消耗所有正面能力，所以影画（抽卡遗物）的额外效果
    /// 不能做成能力，而是在这里由遗物自己结算。
    /// </summary>
    public interface IMingxinJingEnterListener
    {
        /// <param name="choiceContext">结算上下文</param>
        /// <param name="owner">进入明心镜的角色</param>
        /// <param name="consumedPowerCount">本次进入明心镜时被消耗的正面能力种类数</param>
        Task OnMingxinJingEntered(PlayerChoiceContext choiceContext, Creature owner, int consumedPowerCount);
    }
}
