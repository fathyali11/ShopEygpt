namespace Web.Entites.ViewModels.WishlistVMs;
public record AddWishlistItem(
    int ProductId,
    string ProductName,
    string ImageName,
    decimal Price,
    bool IsInWishlist
    );
