namespace MyCCompiler.Analysis
{
    public class Symbol
    {
        public string Lexme { get; set; }
        public Primitive Type { get; set; }

        public override string ToString()
        {
            return $"({Lexme}, {Type})";
        }
    }
}
