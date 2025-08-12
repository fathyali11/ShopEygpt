namespace Web.Entites.ViewModels.OrderVMs;
public record OrderResponseVM(
    int Id ,
    string UserId,
    string UserName,
    string Status
    );