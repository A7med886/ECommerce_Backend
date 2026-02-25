namespace ECommerce.API.Helper
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IdempotentAttribute : Attribute 
    { 

    }
}
