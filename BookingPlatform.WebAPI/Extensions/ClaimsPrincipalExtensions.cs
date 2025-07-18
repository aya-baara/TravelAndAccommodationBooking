using System.Security.Claims;

namespace BookingPlatform.WebAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID claim is missing.");

            return Guid.Parse(userId);
        }

    }
}
