﻿/*

See :
https://www.universal-robots.com/how-tos-and-faqs/how-to/ur-how-tos/real-time-data-exchange-rtde-guide-22229/ 
 
BOOL : bool
UINT8 : byte
UINT32 : uint
UINT64 : ulong
INT32 : int
DOUBLE : double
VECTOR3D : double[]
VECTOR6D : double []
VECTOR6INT32 : int[]
VECTOR6UINT32 : uint[]
  
TODO and not TODO : do not declare public fields with other types & creates the array with the right size

*/

namespace ConsoleAppUR.URrobot
{
    [Serializable]
    public class UniversalRobot_Outputs
    {
        // public double io_current; // check the fields name in the RTDE guide : MUST be the same with the same type
        public double[] actual_q = new double[6]; // array creation must be done here to give the size
        public double[] actual_TCP_pose = new double[6]; // array creation must be done here to give the size
        public double output_double_register_24; //TESTJ1
        public double output_double_register_25;
        public double output_double_register_26;
        public double output_double_register_27;
        public double output_double_register_28;
        // public int robot_mode;

        // free private & protected attributs are allows
        // all properties and methods also (even public) 
    }

    [Serializable]
    public class UniversalRobot_Inputs
    {
        public double input_double_register_20;
        public double input_double_register_21;
        public double input_double_register_22;
        public double input_double_register_23;
        public double input_double_register_24;
        public double input_double_register_25;
    }
}