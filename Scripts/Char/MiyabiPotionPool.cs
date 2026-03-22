using BaseLib.Abstracts;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class MiyabiPotionPool : CustomPotionPoolModel
    {
        public override Color LabOutlineColor => Miyabi.Color;

        public override string EnergyColorName => "ironclad";


        //public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
        //public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
    }
}
