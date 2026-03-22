using BaseLib.Abstracts;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class MiyabiCardPool : CustomCardPoolModel
    {
        public override string Title => Miyabi.CharacterId; //This is not a display name.

        public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        public override string TextEnergyIconPath => "res://images/charui/text_energy.png";

        public override string EnergyColorName => "blue";
        public override string CardFrameMaterialPath => "card_frame_blue";

        //Color of small card icons
        public override Color DeckEntryCardColor => new("4682B4");
        public override bool IsColorless => false;


        /* These HSV values will determine the color of your card back.
        They are applied as a shader onto an already colored image,
        so it may take some experimentation to find a color you like.
        Generally they should be values between 0 and 1. */
        public override float H => 1f; //Hue; changes the color.
        public override float S => 1f; //Saturation
        public override float V => 1f; //Brightness

        //Alternatively, leave these values at 1 and provide a custom frame image.
        /*public override Texture2D CustomFrame(CustomCardModel card)
        {
            //This will attempt to load CharMod/images/cards/frame.png
            return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
        }*/

        
    }
}
