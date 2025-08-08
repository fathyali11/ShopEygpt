using Web.Entites.Models;

namespace Web.Entites.ViewModels.WishlistVMs;

public record WishlistResponse(
     int Id,
     List<WishlistItem> Items
    );
