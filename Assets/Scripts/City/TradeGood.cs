[System.Serializable]
public class TradeGood
{
    public ResourceSO resource;
    public float amount;
    public float price;

    public TradeGood(ResourceSO resource, float amount, float price)
    {
        this.resource = resource;
        this.amount = amount;
        this.price = price;
    }
}