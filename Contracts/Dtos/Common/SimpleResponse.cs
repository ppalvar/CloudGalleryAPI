namespace Contracts.Dtos.Common;

public class SimpleResponse<ItemType>
{
    public string SecurityToken { get; set; }
    public DateTime TokenExpiration { get; set; }
    public ItemType Content { get; set; }

    public SimpleResponse(string securityToken, DateTime tokenExpiration, ItemType content)
    {
        SecurityToken = securityToken;
        Content = content;
        TokenExpiration = tokenExpiration;
    }
}