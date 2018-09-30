namespace AspNetCore.WebApi.ExceptionHandling.Controllers
{
    public class Context
    {
        public Context()
        {
        }

        public Context(string author, string machine)
        {
            this.Author = author;
            this.Machine = machine;
        }

        public string Author { get; set; }

        public string Machine { get; set; }

        public override string ToString()
        {
            return $"author = {this.Author} @ machine = {this.Machine}";
        }
    }
}