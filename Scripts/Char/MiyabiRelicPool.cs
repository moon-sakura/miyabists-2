using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class MiyabiRelicPool : TypeListRelicPoolModel
    {
        public override Color LabOutlineColor => Miyabi.Color;

        public override string EnergyColorName => "defect";

        public override string BigEnergyIconPath => "defect";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";
    }
}
