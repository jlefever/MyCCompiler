int fib(int n)
{
    if (n < 2)
    {
        return n;
    }

    return fib(n - 1) + fib(n - 2);
}

int main()
{
    int n = 10;
    printf("The number is %d", fib(n));
    return 0;
}
