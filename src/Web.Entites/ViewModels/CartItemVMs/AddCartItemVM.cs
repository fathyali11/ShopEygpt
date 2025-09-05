namespace Web.Entites.ViewModels.CartItemVMs;
public record AddCartItemVM(
    int ProductId,
    string ProductName,
    string ImageName,
    decimal Price,
    int Count = 1
    );
