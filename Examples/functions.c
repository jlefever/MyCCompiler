int add_6(int a, int b)
{
    int sum = a + b;
    int six = 6;

    {
        int sum = 4;
        printf("Our nested sum in %d", sum);
    }

    printf("We add 6 to %d\n", sum);

    return a + b + six;
}

int main()
{
    int total = add_6(add_6(5, 7), 4) + 2;
    printf("The total is %d", total);
}