namespace LostButFound.API.Domian
{
    public class Thing
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PathToIMG { get; set; }

        public int IsLost { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Street { get; set; }

        public string Metro { get; set; }

        public int IsApproved { get; set; }

       
    }
}
