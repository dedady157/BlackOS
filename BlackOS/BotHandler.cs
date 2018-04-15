using Ev3DevLib;
using Ev3DevLib.Motors;
using Ev3DevLib.Sensors;

namespace BlackOS
{
    public static class BotHandler
    {
        public static Device _A => BrickControll.OutputPorts.OutA;
        public static Device _B => BrickControll.OutputPorts.OutB;
        public static Device _C => BrickControll.OutputPorts.OutC;
        public static Device _D => BrickControll.OutputPorts.OutD;
        public static Device _1 => BrickControll.InputPorts.In1;
        public static Device _2 => BrickControll.InputPorts.In2;
        public static Device _3 => BrickControll.InputPorts.In3;
        public static Device _4 => BrickControll.InputPorts.In4;

        public static void ChangeLEDsTo(LEDColor color)
        {
            BrickControll.Brick.LEDS.SetBothLedsTo(color);
        }
    }
}
