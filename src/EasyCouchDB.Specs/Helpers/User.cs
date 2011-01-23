namespace EasyCouchDB.Specs.Helpers
{
    public class User : Document<string>
    {
        public string Fullname { get; set; }
        public string EmailAddress { get; set; }
    }
}