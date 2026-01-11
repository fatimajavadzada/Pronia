namespace Pronia.Abstraction
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemsAsync();
    }
}
