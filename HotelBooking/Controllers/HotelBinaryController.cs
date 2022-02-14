using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;
using System.Text.Json;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelBinaryController : ControllerBase
    {
        public static Dictionary<int, Model> Hotel = new Dictionary<int, Model>();

        public Stack<NodeTree> stN = new Stack<NodeTree>();

        private readonly ILogger<HotelBinaryController> _logger;

        public HotelBinaryController(ILogger<HotelBinaryController> logger)
        {
            _logger = logger;
        }

        [HttpGet("rooms-layout")]
        public Array LoadRoomsLayout()
        {
            if(! (Hotel.Count > 0))
            {
                Hotel = HotelSetup(4, 5);
            }
            return Hotel.Select(x => Tuple.Create(x.Key, (x.Value.FloorDetail.Select(y => Tuple.Create(y.Key, y.Value)).ToArray()))).ToArray();
        }
        public Dictionary<int, Model> HotelSetup(int NoOfFloors, int NoOfRoomsPerFloor)
        {
            //Room status
            //A= Available
            //O= Occupied
            //V= Vacant
            //R= Repair

            //Floor Status
            //A= Available
            //NA= Not Available

            //New Hotel setup , by default "A" - Available (for Room and Floor)

            //Blueprint:
            //Even Number Floor follows EDCBA
            //Odd Number Floor follows ABCDE

            Dictionary<int, Model> HotelOut = new Dictionary<int, Model>();
            string DefaultStatusOfRoom = "A";
            string DefaultStatusOfFloor = "A";

            string RoomNames = GenerateRoomNames(NoOfRoomsPerFloor);
            for (int floornum = 1; floornum <= NoOfFloors; floornum++)
            {
                Dictionary<string, string> floordetail = new Dictionary<string, string>();
                if ((floornum % 2) == 0)
                {
                    char[] strarray = RoomNames.ToCharArray();
                    Array.Reverse(strarray);
                    foreach (char name in new String(strarray))
                    {
                        floordetail.Add(floornum + name.ToString(), DefaultStatusOfRoom);
                    }
                }
                else
                {
                    foreach (char name in RoomNames)
                    {
                        floordetail.Add(floornum + name.ToString(), DefaultStatusOfRoom);
                    }
                }
                HotelOut.Add(floornum, new Model(floordetail, DefaultStatusOfFloor));
            }
            return HotelOut;
        }
        public string GenerateRoomNames(int NoOfRoomsPerFloor)
        {
            string RoomName = "";
            for (int i = 65; (i < (65 + NoOfRoomsPerFloor) && i <= 90); i++)
            {
                // Convert the int to a char to get the actual character behind the ASCII code
                RoomName += ((char)i).ToString();
            }
            return RoomName;
        }

        [HttpPost("update-room")]
        public string UpdateRoom(updateRoomRequest updateRoom)
        {
            try
            {
                //repaircompleted = false - default - No action performed
                //repairroom = true  - Repair Completed

                string Message = "Unable to Perform action, Contact System Admin";
                var floor = Hotel[(int)Char.GetNumericValue(updateRoom.roomname[0])];

                if (updateRoom.roomstatus == "O")// Make Booking
                {
                    if (floor.FloorDetail[updateRoom.roomname] == "A")
                    {
                        floor.FloorDetail[updateRoom.roomname] = updateRoom.roomstatus;
                        Message = "Requested Room: " + updateRoom.roomname + " Booked Successfully";
                    }
                }
                else if (updateRoom.roomstatus == "V")//Make Vacant
                {
                    if (updateRoom.repaircompleted == true)//Make Vacant after Room Repaired
                    {
                        if (floor.FloorDetail[updateRoom.roomname] == "R")
                        {
                            floor.FloorDetail[updateRoom.roomname] = updateRoom.roomstatus;
                            Message = "Requested Room: " + updateRoom.roomname + " Repair Completed, Becomes Vacant";
                        }
                    }
                    else //Make Vacant after guest checkout
                    {
                        if (floor.FloorDetail[updateRoom.roomname] == "O")
                        {
                            floor.FloorDetail[updateRoom.roomname] = updateRoom.roomstatus;
                            Message = "Requested Room: " + updateRoom.roomname + " Checked Out, Becomes Vacant";
                        }
                    }
                }
                else if (updateRoom.roomstatus == "A")//Cleaning Completed
                {
                    if (floor.FloorDetail[updateRoom.roomname] == "V")
                    {
                        floor.FloorDetail[updateRoom.roomname] = updateRoom.roomstatus;
                        Message = "Requested Room: " + updateRoom.roomname + " Cleaning Completed, Becomes Available";
                    }
                }
                else if (updateRoom.roomstatus == "R")//Room going to be repair
                {
                    if (floor.FloorDetail[updateRoom.roomname] == "V")
                    {
                        floor.FloorDetail[updateRoom.roomname] = updateRoom.roomstatus;
                        Message = "Requested Room: " + updateRoom.roomname + " Ready to Repair.";
                    }
                }

                floor.FloorAvail = "NA";
                foreach (var floorstatusobj in floor.FloorDetail.Values)
                {
                    if (floorstatusobj == "A")
                    {
                        floor.FloorAvail = floorstatusobj;
                        break;
                    }
                }
                return Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
       
        [HttpGet("search-nearest")]
        public string SearchNearestRoom()
        {
            try
            {
                string availableroom = "";

                var availfloor = new Model(new Dictionary<string, string>(), "");
                for (int i = 1; i <= Hotel.Count; i++)
                {
                    if (Hotel[i].FloorAvail == "A")
                    {
                        availfloor = Hotel[i];
                        break;
                    }

                }

                foreach (var room in availfloor.FloorDetail)
                {
                    if (room.Value == "A")
                    {
                        availableroom = room.Key;
                        break;
                    }
                }
                if (availableroom != "")
                {
                    return "The Nearest Room is  : " + availableroom;
                }
                else
                {
                    return "No Room Avaiilable for Now, Check again later.";
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

        //[HttpGet("binary-evaluation")]
        //async public void BinaryTreeEvaluation()
        //{
        //    string given = "((15 / (7 - (1 + 1)) ) * -3 ) -(2 + (1 + 1))";
        //    given = given.Replace(" ", String.Empty);

        //    given = "(" + given;
        //    given += ")";
        //    var node = await build(given);
        //}

        //public static int evalTree(NodeTree root)
        //{

        //    if (root == null)
        //        return 0;

        //    if (root.left == null && root.right == null)
        //        return Convert.ToInt32((root.data));

        //    int leftEval = evalTree(root.left);

        //    int rightEval = evalTree(root.right);

        //    if (root.data.Equals("+"))
        //        return leftEval + rightEval;

        //    if (root.data.Equals("-"))
        //        return leftEval - rightEval;

        //    if (root.data.Equals("*"))
        //        return leftEval * rightEval;

        //    if (root.data.Equals("/"))
        //        return leftEval / rightEval;

        //    return leftEval / rightEval;
        //}

       
        //static NodeTree newNode(string c)
        //{
        //    NodeTree n = new NodeTree();
        //    n.data = c;
        //    n.left = n.right = null;
        //    return n;
        //}

        //async Task<NodeTree> build(string s)
        //{ 
        //    Stack<char> stC = new Stack<char>();
        //    NodeTree t, t1, t2;

        //    int[] p = new int[123];
        //    p['+'] = p['-'] = 1;
        //    p['/'] = p['*'] = 2;
        //    p[')'] = 0;
        //    StringBuilder _number = new StringBuilder();
        //    Boolean negativeNumberFound = false;
        //    Boolean doNotPop = false;
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        if (s[i] == '-')
        //        {
        //            if (char.IsNumber(s[i + 1]))
        //            {
        //                negativeNumberFound = true;
        //            }
        //        }

        //        if (s[i] == '(')
        //        {
        //            stC.Push(s[i]);

        //        }


        //        else if (char.IsNumber(s[i]))
        //        {
        //            _number.Append(s[i]);

        //        }
        //        else if (p[s[i]] > 0)
        //        {
        //            Boolean handleNegative = false;
        //            Boolean processNegativeValues = false;
        //            if (_number.ToString() != "")
        //            {
        //                var currentChar = s[i];
        //                var previousNumber = _number;
        //                int indexToTrack = i - (s[i].ToString().Length + _number.Length + 1);
        //                if (indexToTrack >= 0)
        //                {
        //                    var charPreviousToNumber = s[indexToTrack];
        //                    if (!char.IsNumber(charPreviousToNumber))
        //                    {
        //                        processNegativeValues = true;
        //                    }
        //                }
        //                while (negativeNumberFound == true && stC.Count > 1 && processNegativeValues == true && stC.Peek().Equals('-'))
        //                {
        //                    handleNegative = true;
        //                    t = stN.Peek();
        //                    t2 = t.right;
        //                    t.left = null;
        //                    t.right = null;

        //                    if (stN.Count > 0)
        //                    {
        //                        t1 = newNode(stC.Peek().ToString());
        //                        stC.Pop();
        //                    }
        //                    else
        //                    {
        //                        t1 = null;
        //                    }
        //                    stN.Pop();
        //                    t.left = t2;
        //                    t.right = t1;
        //                    stN.Push(t);

        //                    t = newNode(_number.ToString());
        //                    stN.Push(t);
        //                    _number.Clear();

        //                    if (stC.Count != 0 && stC.Peek() == '(')
        //                    {
        //                        if (stN.Count > 0)
        //                        {
        //                            t1 = stN.Peek();
        //                            stN.Pop();
        //                        }
        //                        else
        //                        {
        //                            t1 = null;
        //                        }

        //                        t = stN.Peek();
        //                        t2 = t.left;
        //                        t.left = null;

        //                        t.left = t2;
        //                        t.right.right = t1;
        //                        stN.Push(t);
        //                        stC.Pop();
        //                        doNotPop = true;

        //                    }
        //                    negativeNumberFound = false;
        //                }
        //                if (handleNegative == false)
        //                {
        //                    t = newNode(_number.ToString());
        //                    stN.Push(t);
        //                    _number.Clear();
        //                }
        //            }

        //            while (stC.Count != 0 && stC.Peek() != '(' && p[stC.Peek()] >= p[s[i]])
        //            {

        //                t = newNode(stC.Peek().ToString());
        //                stC.Pop();
        //                t1 = stN.Peek();
        //                stN.Pop();
        //                if (stN.Count > 0)
        //                {
        //                    t2 = stN.Peek();
        //                    stN.Pop();
        //                }
        //                else
        //                {
        //                    t2 = null;
        //                }
        //                t.left = t2;
        //                t.right = t1;
        //                stN.Push(t);
        //            }
        //            stC.Push(s[i]);
        //            _number.Clear();
        //        }
        //        else if (s[i] == ')')
        //        {
        //            Boolean handleNegative = false;
        //            Boolean processNegativeValues = false;
        //            if (_number.ToString() != "")
        //            {
        //                // )-4)    16,17,18,19
        //                var currentChar = s[i];
        //                var previousNumber = _number;
        //                int indexToTrack = i - (s[i].ToString().Length + _number.Length + 1);
        //                if (indexToTrack >= 0)
        //                {
        //                    var charPreviousToNumber = s[indexToTrack];
        //                    if (!char.IsNumber(charPreviousToNumber))
        //                    {
        //                        processNegativeValues = true;
        //                    }
        //                }
        //                while (negativeNumberFound == true && stC.Count > 1 && processNegativeValues == true && stC.Peek().Equals('-'))
        //                {
        //                    handleNegative = true;
        //                    t = stN.Peek();
        //                    t2 = t.right;
        //                    t.left = null;
        //                    t.right = null;

        //                    if (stN.Count > 0)
        //                    {
        //                        t1 = newNode(stC.Peek().ToString());
        //                        stC.Pop();
        //                    }
        //                    else
        //                    {
        //                        t1 = null;
        //                    }
        //                    stN.Pop();
        //                    t.left = t2;
        //                    t.right = t1;
        //                    stN.Push(t);

        //                    t = newNode(_number.ToString());
        //                    stN.Push(t);
        //                    _number.Clear();

        //                    if (stC.Count != 0 && stC.Peek() == '(')
        //                    {
        //                        if (stN.Count > 0)
        //                        {
        //                            t1 = stN.Peek();
        //                            stN.Pop();
        //                        }
        //                        else
        //                        {
        //                            t1 = null;
        //                        }

        //                        t = stN.Peek();
        //                        t2 = t.left;
        //                        t.left = null;

        //                        t.left = t2;
        //                        t.right.right = t1;
        //                        stN.Push(t);
        //                        stC.Pop();
        //                        doNotPop = true;

        //                    }
        //                    negativeNumberFound = false;
        //                }
        //                if (handleNegative == false)
        //                {
        //                    t = newNode(_number.ToString());
        //                    stN.Push(t);
        //                    _number.Clear();
        //                }
        //            }
        //            while (stC.Count != 0 && stC.Peek() != '(')
        //            {
        //                t = newNode(stC.Peek().ToString());
        //                stC.Pop();

        //                if (stN.Count > 0)
        //                {
        //                    t1 = stN.Peek();
        //                    stN.Pop();
        //                }
        //                else
        //                {
        //                    t1 = null;
        //                }
        //                if (stN.Count > 0)
        //                {
        //                    t2 = stN.Peek();
        //                    stN.Pop();
        //                }
        //                else
        //                {
        //                    t2 = null;
        //                }

        //                t.left = t2;
        //                t.right = t1;
        //                stN.Push(t);
        //                _number.Clear();
        //            }
        //            if (stC.Count != 0 && doNotPop == false)
        //            {
        //                stC.Pop();

        //            }
        //            else
        //            {
        //                doNotPop = false;
        //            }

        //        }
        //    }
        //    t = stN.Peek();
        //    return t;
        //}
    }
}