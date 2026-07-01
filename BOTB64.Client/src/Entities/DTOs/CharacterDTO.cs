namespace BOTB64.Entities.DTOs
{
    public class CharacterDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ModelURI { get; set; }
        public string ScriptURI { get; set; }
        public string IconURI { get; set; }
        public bool Enabled { get; set; }
    }
}
