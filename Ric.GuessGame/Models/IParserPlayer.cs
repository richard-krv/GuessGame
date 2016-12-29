namespace Ric.GuessGame.Models
{
    public interface IParserPlayer
    {
        string Name { get; set; }
        string Type { get; set; }
    }

    public class ParserPlayer : IParserPlayer
    {
        public string Name
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
    }
}
