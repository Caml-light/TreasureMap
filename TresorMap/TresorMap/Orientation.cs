using System;
using System.ComponentModel;

namespace TreasureMap
{
    public enum Orientation
    {
        [Description("Nord")]
        N,
        [Description("Est")]
        E,
        [Description("Sud")]
        S,
        [Description("Ouest")]
        O,
    }

    public static class OrientationHelper
    {
        public static Orientation Turn(this Orientation orientation, char mouv)
        {
            bool isRight = mouv == 'D';
            if (!isRight && mouv != 'G')
            {
                throw new ArgumentException( "Parsing Error : number of parameter for Mountain not correct");
            }

            switch (orientation)
            {
                case Orientation.N:
                    return isRight ? Orientation.E : Orientation.O;
                case Orientation.E:
                    return isRight ? Orientation.S : Orientation.N;
                case Orientation.S:
                    return isRight ? Orientation.O : Orientation.E;
                case Orientation.O:
                default:
                    return isRight ? Orientation.N : Orientation.S;
            }
        }

        public static Orientation ParseOrientation(char or)
        {
            switch (or)
            {
                case 'N':
                    return Orientation.N;
                case 'E':
                    return Orientation.E;
                case 'S':
                    return Orientation.S;
                case 'O':
                    return Orientation.O;
                default:
                    throw new ArgumentException("ParseOrientation : char args not recognized");
            }
        }
        public static (int xPos, int yPos) NextPos(this Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.N:
                    return (0, -1);
                case Orientation.E:
                    return (1, 0);
                case Orientation.S:
                    return (0, 1);
                case Orientation.O:
                default:
                    return (-1, 0);
            }
        }
    }


}
