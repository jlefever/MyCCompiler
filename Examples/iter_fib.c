int fib(int n)
{
    int tmp, i = 1, f = 1, f_prev = 0;

    do
    {
        printf("F_%d is %d\n", i, f);
        tmp = f;
        f = f + f_prev;
        f_prev = tmp;
        i = i + 1;
    } while (i < n + 1);
}

int main()
{
    int n;

    printf("Enter n: ");
    scanf("%d", &n);

    printf("The digit is %d\n", n);

    fib(n);
}