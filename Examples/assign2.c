#include <stdio.h>

// Illegal
// Should this fail when building AST or during semantic analysis
// int foo() = 4;

int main()
{
	int a, b = 1;
	int c = 2, d = 3;
	int e = 4, f;

	printf("a: %d\n", a);
	printf("b: %d\n", b);
	printf("c: %d\n", c);
	printf("d: %d\n", d);
	printf("e: %d\n", e);
	printf("f: %d\n", f);
}