using System.Security.Claims;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        public string? GetStoreId() =>
            _http.HttpContext?.User?.FindFirst("StoreId")?.Value;

        public string? GetUserId() =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? GetRole() =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsInRole(string role) =>
                _http.HttpContext?.User?.IsInRole(role) ?? false;
    }
}