using WM.ProductsApi.Models;

namespace WM.ProductsApi.Data;

public static class SeedData
{
    // Use these buyers verbatim per the assessment
    public static readonly List<Buyer> Buyers = new()
    {
        new Buyer
        {
            Id = "49ec2a8703224eea9dec16b22546477e",
            Name = "Johnny Buyer",
            Email = "jbuyer@test.com"
        },
        new Buyer
        {
            Id = "a790a7b6bf2a48569066c46306c3332d",
            Name = "Jennie Purchaser",
            Email = "jpurchaser@test.com"
        }
    };
}
