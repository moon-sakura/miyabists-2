using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class MiyabiCardPool : TypeListCardPoolModel
    {
        public override string Title => Miyabi.CharacterId; //This is not a display name.
        
        public override string BigEnergyIconPath => "defect";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";

        public override string EnergyColorName => "defect";

        public override string CardFrameMaterialPath => "card_frame_blue";

        //Color of small card icons
        public override Color DeckEntryCardColor => new("4682B4");
        public override bool IsColorless => false;

    }
}
