namespace MyCCompiler.Analysis
{
    public class Symbol
    {
        public string Lexme { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"({Lexme}, {Type})";
        }
    }
}
