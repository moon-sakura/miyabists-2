using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rooms;
using Miyabists2.Scripts.Char;
using Miyabists2.Scripts.Relics;
using Miyabists2.Scripts.Service;
using STS2RitsuLib.Interop.AutoRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Cards
{
    [RegisterCard(typeof(StatusCardPool))]
    internal class ChangYeShipian : MiyabiCardBase
    {
        protected override string ArtPath => "res://images/cards/changyeshipian.png";
        //public override CardAssetProfile AssetProfile => new(
        //    PortraitPath: "res://images/cards/commonCards.png",
        //    FramePath: ImageHelper.GetImagePath("atlases/card_atlas.sprites/beta.tres"),
        //    AncientBorderPath : ImageHelper.GetImagePath("atlases/compressed_atlas.sprites/ancient_card_border.png.tres"),
        //    AncientTextBgPath: ImageHelper.GetImagePath("atlases/compressed_atlas.sprites/ancient_text_bg_" + base.Type.ToString().ToLowerInvariant() + ".png.tres"),
        //    BannerTexturePath : ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_banner_ancient_s.tres"),
        //    BannerMaterialPath : "res://materials/cards/banners/card_banner_ancient_mat.tres"
        //    );

        public override int MaxUpgradeLevel => 0;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
        ];

        public ChangYeShipian()
            : base(0, CardType.Power, CardRarity.Ancient, TargetType.AnyEnemy, false)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Target == null || !cardPlay.Target.IsEnemy) return;

            var relic = (TonghuaJishibenRelic)MiyabiFuncBase.GetRelic<TonghuaJishibenRelic>(Owner);
            if (relic != null)
            {
                relic.RecordMonster(cardPlay.Target);
            }
        }
    }
}
