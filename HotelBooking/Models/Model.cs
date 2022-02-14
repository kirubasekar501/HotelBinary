namespace HotelBooking.Models
{
    public class Model
    {
        public Model(Dictionary<string, string> _floorDetail, string _floorAvail)
        {
            this.FloorDetail = _floorDetail;
            this.FloorAvail = _floorAvail;
        }
        public string FloorAvail { get; set; }

        public Dictionary<string, string> FloorDetail { get; set; }
    }
}
