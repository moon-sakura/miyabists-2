using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class YixuanCardPool : TypeListCardPoolModel
    {
        public override string Title => Yixuan.CharacterId; //This is not a display name.

        public override string BigEnergyIconPath => "regent";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";

        public override string EnergyColorName => "regent";

        // 金色卡框
        public override string CardFrameMaterialPath => "card_frame_gold";

        //Color of small card icons - 墨金色
        public override Color DeckEntryCardColor => new("8B7539");
        public override bool IsColorless => false;
    }
}