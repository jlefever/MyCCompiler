int main()
{
    int a = 3;
    int b = 4;

    printf("&a is %d\n", &a);
    printf("&b is %d\n", &b);

    int *p = &a - 4;

    printf(" p is %d\n", p);
    printf("&p is %d\n", &p);
    printf("*p is %d\n", *p);
}