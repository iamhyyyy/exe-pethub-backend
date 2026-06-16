namespace EXE_PET_HUB.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetStoreId();
        string? GetUserId();
        string? GetRole();
        bool IsInRole(string role);
    }
}