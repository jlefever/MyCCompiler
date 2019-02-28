namespace MyCCompiler.Analysis
{
    public class Symbol
    {
        public string Lexme { get; set; }
        public IType Type { get; set; }

        public override string ToString()
        {
            return $"({Lexme}, {Type})";
        }
    }
}
