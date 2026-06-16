using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using Miyabists2.Scripts.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Patches
{
    // ================= 补丁 1：拦截 UI 图标渲染 =================
    [HarmonyPatch(typeof(NRelicCollectionCategory), "LoadRelicNodes")]
    public class HideRelicFromUIPatch
    {
        // 使用 ref IEnumerable<RelicModel> 可以在游戏读取列表前，直接用 LINQ 偷天换日
        static void Prefix(ref IEnumerable<RelicModel> relics)
        {
            if (relics == null) return;

            if (MiyabiModConfig.ShowSameRelic) return;

            // 🎯 过滤掉你不想在图鉴看到的那个仪玄重复包装类 ID
            relics = relics.Where(r => r != null && r is not ISharedType);

            // 如果你有多个想屏蔽的，可以用哈希表或者 Contains 判定：
            // var hiddenIds = new HashSet<string> { "ID_1", "ID_2" };
            // relics = relics.Where(r => r != null && !hiddenIds.Contains(r.Id));
        }
    }

    // ================= 补丁 2：拦截详情页轮播列表 =================
    [HarmonyPatch(typeof(NRelicCollection), nameof(NRelicCollection.AddRelics))]
    public class HideRelicFromCarouselPatch
    {
        static void Prefix(ref IEnumerable<RelicModel> relics)
        {
            if (relics == null) return;

            if (MiyabiModConfig.ShowSameRelic) return;

            // 🎯 这里的过滤条件必须和上面完全一致
            // 确保玩家点开相邻遗物详情页、左右切换时，不会“穿帮”切换到被隐藏的遗物上
            relics = relics.Where(r => r != null && r is not ISharedType);
        }
    }
}
