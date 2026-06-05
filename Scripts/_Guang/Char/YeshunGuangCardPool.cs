using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class YeshunGuangCardPool : TypeListCardPoolModel
    {
        public override string Title => YeshunGuang.CharacterId; //This is not a display name.

        public override string BigEnergyIconPath => "defect";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";

        public override string EnergyColorName => "defect";

        // 粉色卡框
        public override string CardFrameMaterialPath => "card_frame_pink";

        //Color of small card icons - 白粉色
        public override Color DeckEntryCardColor => new("E8B4C8");
        public override bool IsColorless => false;
    }
}
