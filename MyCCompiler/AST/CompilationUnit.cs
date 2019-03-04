﻿using System.Collections.Generic;

namespace MyCCompiler.AST
{
    public interface INode { }

    public interface IExternal : INode { }

    public class CompilationUnit : INode
    {
        public IList<IExternal> Externals { get; }

        public CompilationUnit(IList<IExternal> externals)
        {
            Externals = externals;
        }
    }

    public class FunctionDefinition : IExternal
    {
        public string Text { get; }

        public FunctionDefinition(string text)
        {
            Text = text;
        }
    }
}
