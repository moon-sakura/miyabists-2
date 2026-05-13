using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Miyabists2.Scripts.Char;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Bangboo.BangbooRelic
{
    [Pool(typeof(MiyabiRelicPool))]
    internal class MiyabiBangbooRelicBase : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.None;
        public override string PackedIconPath => "res://images/bangboo/relicMode/eousRelic.png";
        protected override string PackedIconOutlinePath => PackedIconPath;
        protected override string BigIconPath => PackedIconPath;
        protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();
    }
}
