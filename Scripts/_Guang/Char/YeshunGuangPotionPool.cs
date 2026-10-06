using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class YeshunGuangPotionPool : TypeListPotionPoolModel
    {
        public override Color LabOutlineColor => YeshunGuang.Color;

        public override string EnergyColorName => "ironclad";

        public override string BigEnergyIconPath => "ironclad";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";
    }
}
